import os
import logging
from typing import List
from langchain_community.vectorstores import Chroma
from langchain_openai import OpenAIEmbeddings

from langchain.schema import Document

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class VectorDB:
    def __init__(self, openai_api_key: str, persist_directory: str = "./chroma_db"):
        """Khởi tạo Vector Database với ChromaDB"""
        self.openai_api_key = openai_api_key
        self.persist_directory = persist_directory
        self.embeddings = OpenAIEmbeddings(api_key=self.openai_api_key)
        
        # Đảm bảo thư mục tồn tại
        os.makedirs(persist_directory, exist_ok=True)
        
        try:
            # Khởi tạo ChromaDB client
            from chromadb import PersistentClient
            self.chroma_client = PersistentClient(path=persist_directory)
            logger.info(f"ChromaDB client đã được khởi tạo tại1 {persist_directory}")
        except Exception as e:
            logger.error(f"Lỗi khi khởi tạo ChromaDB client: {e}")
            self.chroma_client = None
        

    def collection_exists(self, name: str) -> bool:
        """Kiểm tra collection có tồn tại không"""
        try:
            # Liệt kê tất cả collection
            collections = self.chroma_client.list_collections()
            # So sánh không phân biệt chữ hoa/thường
            return any(c.name.lower() == name.lower() for c in collections)
        except Exception as e:
            logger.error(f"Lỗi khi kiểm tra collection: {str(e)}")
            return False

    def add_documents(self, documents: List[Document], collection_name: str = "default") -> str:
        """
        Thêm tài liệu vào database
        """
        try:
            # Sử dụng langchain Chroma với collection_name
            collection = Chroma.from_documents(
                documents=documents,
                embedding=self.embeddings,
                persist_directory=self.persist_directory,
                collection_name=collection_name
            )
            collection.persist()
            logger.info(f"Đã thêm {len(documents)} tài liệu vào collection '{collection_name}'")
            return collection_name
        except Exception as e:
            logger.error(f"Lỗi khi thêm tài liệu: {e}")
            raise


    def similarity_search(self, query: str, collection_name: str = "default", k: int = 5, score_threshold: float = 0.5) -> List[Document]:
        """
        Tìm kiếm các tài liệu tương tự với query với ngưỡng điểm tương đồng
        """
        try:
            if not self.collection_exists(collection_name):
                logger.warning(f"Collection '{collection_name}' không tồn tại")
                return []

            db = Chroma(
                persist_directory=self.persist_directory,
                embedding_function=self.embeddings,
                collection_name=collection_name
            )
            
            # Sử dụng similarity_search_with_score để lấy cả điểm tương đồng
            results_with_scores = db.similarity_search_with_score(query, k=k)
            
            # Lọc kết quả dựa trên ngưỡng điểm
            filtered_results = []
            for doc, score in results_with_scores:
                # Lưu ý: Chroma thường trả về khoảng cách, không phải điểm tương đồng
                # Nên có thể bạn cần điều chỉnh công thức dưới đây tùy thuộc vào cách tính của Chroma
                similarity = 1.0 - score  # Chuyển khoảng cách thành điểm tương đồng
                
                if similarity >= score_threshold:
                    filtered_results.append(doc)
                    logger.info(f"Kết quả phù hợp (điểm: {similarity:.4f}): {doc.page_content[:100]}...")
                else:
                    logger.info(f"Loại bỏ kết quả không phù hợp (điểm: {similarity:.4f})")
            
            # logger.info(f"Đã tìm thấy {len(filtered_results)} kết quả phù hợp cho query: '{query}'")
            
            # # In ra các kết quả
            # if filtered_results:
            #     print(f"Đã tìm thấy {len(filtered_results)} kết quả cho query: '{query}'")
            #     print("Kết quả tìm kiếm:")
            #     for result in filtered_results:
            #         print(result)
            # else:
            #     print(f"Không tìm thấy kết quả phù hợp cho query: '{query}'")
            
            return filtered_results
        except Exception as e:
            logger.error(f"Lỗi khi tìm kiếm: {e}")
            return []

    def get_relevant_context(self, query: str, collection_name: str = "default", k: int = 5, score_threshold: float = 0.5) -> str:
        """
        Lấy ngữ cảnh liên quan từ tài liệu để sử dụng cho prompt với ngưỡng điểm tương đồng
        """
        documents = self.similarity_search(query, collection_name, k, score_threshold)

        if not documents:
            logger.warning(f"Không tìm thấy tài liệu nào phù hợp với query: {query}")
            return ""

        context = "\n\n".join([doc.page_content for doc in documents])
        #logger.info(f"Đã lấy được ngữ cảnh từ vector DB ({len(context)} ký tự)")
        return context
    
    # def get_vectorstore(collection_name: str = None) -> Chroma:
    #     """Lấy vector store theo tên collection"""
    #     embedding_function = OpenAIEmbeddings()
        
    #     if collection_name is None:
    #         logger.warning("Không có collection_name được chỉ định, sử dụng Chroma DB mặc định")
    #         return Chroma(
    #             embedding_function=embedding_function,
    #             persist_directory="./chroma_db"
    #         )
        
    #     logger.info(f"Đã tạo retriever từ ChromaDB tại ./chroma_db với collection {collection_name}")
    #     return Chroma(
    #         collection_name=collection_name,
    #         embedding_function=embedding_function,
    #         persist_directory="./chroma_db"
    #     )
    def delete_collection(self, name: str):
        try:
            self.chroma_client.delete_collection(name)
            logger.info(f"Đã xóa collection: {name}")
        except Exception as e:
            logger.error(f"Lỗi khi xóa collection: {e}")


# Hàm để lấy đối tượng VectorDB duy nhất (Singleton)
vector_db_instance = None

def get_vector_db() -> VectorDB:
    global vector_db_instance
    if vector_db_instance is None:
        vector_db_instance = VectorDB(
            openai_api_key="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",
            persist_directory="./chroma_db"
        )
        logger.info("VectorDB đã được khởi tạo trong vector_db.py.")
    return vector_db_instance