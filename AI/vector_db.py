import chromadb
from langchain_community.vectorstores import Chroma
from langchain_openai import OpenAIEmbeddings

openai_api_key = "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"  # Đặt API Key của bạn
embeddings = OpenAIEmbeddings(api_key=openai_api_key)

chroma_client = chromadb.PersistentClient(path="./chroma_db")
vector_store = None

def initialize_vector_db():
    """Khởi tạo Vector Database"""
    global vector_store
    vector_store = Chroma(persist_directory="./chroma_db", embedding_function=embeddings)

def add_text_to_vector_db(text):
    """Thêm nội dung vào Vector DB"""
    if vector_store is None:
        initialize_vector_db()
    vector_store.add_texts([text])

def search_in_vector_db(query):
    """Tìm kiếm nội dung trong Vector DB"""
    if vector_store is None:
        initialize_vector_db()
    docs = vector_store.similarity_search(query, k=5)  # Tìm 5 nội dung gần nhất
    return " ".join([doc.page_content for doc in docs]) if docs else ""

