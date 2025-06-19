
import os
import logging
from typing import List, Optional
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.schema import Document
from langchain_community.document_loaders import (
    TextLoader,
    PyPDFLoader,
    Docx2txtLoader,
    CSVLoader
)
from langchain_community.document_loaders import PyMuPDFLoader

# OCR
from pdf2image import convert_from_path, pdfinfo_from_path
import gc
import pytesseract

# Nếu dùng Windows:
pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
os.environ["PATH"] += os.pathsep + r"C:\Users\Admin\Downloads\Release-24.08.0-0\poppler-24.08.0\Library\bin"


logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class FileProcessor:
    def __init__(self, chunk_size: int = 1000, chunk_overlap: int = 200):
        self.chunk_size = chunk_size
        self.chunk_overlap = chunk_overlap
        self.text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=chunk_size,
            chunk_overlap=chunk_overlap,
            length_function=len
        )

    def extract_text_from_scanned_pdf(self, file_path: str) -> List[Document]:
        logger.info("🔍 Đang thực hiện OCR file scan từng trang bằng Tesseract...")
        try:
            # Lấy tổng số trang
            info = pdfinfo_from_path(file_path, userpw=None)
            total_pages = info["Pages"]

            full_text = ""
            for i in range(total_pages):
                images = convert_from_path(file_path, dpi=150, first_page=i+1, last_page=i+1)
                image = images[0]
                text = pytesseract.image_to_string(image, lang='eng+vie')
                logger.debug(f"📃 Trang {i + 1} OCR: {len(text)} ký tự")
                full_text += text + "\n"

                # Giải phóng bộ nhớ
                del image
                del images
                gc.collect()

            if not full_text.strip():
                logger.warning("⚠️ Không nhận diện được văn bản từ ảnh (OCR rỗng)")
                return []

            doc = Document(page_content=full_text, metadata={"source": file_path, "ocr": True})
            return [doc]
        except Exception as e:
            logger.error(f"❌ Lỗi OCR với file {file_path}: {e}")
            return []

    def process_file(self, file_path: str, metadata: Optional[dict] = None) -> List[Document]:
        if not os.path.exists(file_path):
            logger.error(f"❌ File không tồn tại: {file_path}")
            return []

        try:
            if metadata is None:
                metadata = {"source": file_path}
            else:
                metadata["source"] = file_path

            file_extension = os.path.splitext(file_path)[1].lower()
            documents = []

            if file_extension == '.pdf':
                try:
                    logger.info("📄 Thử trích xuất với PyPDFLoader...")
                    loader = PyPDFLoader(file_path)
                    documents = loader.load()
                except Exception as e:
                    logger.warning(f"❌ PyPDFLoader lỗi: {e}")

                if not documents or all(len(doc.page_content.strip()) == 0 for doc in documents):
                    try:
                        logger.info("📄 Thử tiếp với PyMuPDFLoader...")
                        loader = PyMuPDFLoader(file_path)
                        documents = loader.load()
                    except Exception as e:
                        logger.warning(f"❌ PyMuPDFLoader lỗi: {e}")

                if not documents or all(len(doc.page_content.strip()) == 0 for doc in documents):
                    logger.warning("🧠 Không có nội dung → dùng OCR")
                    documents = self.extract_text_from_scanned_pdf(file_path)

            elif file_extension in ['.docx', '.doc']:
                loader = Docx2txtLoader(file_path)
                documents = loader.load()
            elif file_extension == '.csv':
                loader = CSVLoader(file_path)
                documents = loader.load()
            elif file_extension in ['.txt', '.md']:
                loader = TextLoader(file_path, encoding='utf-8')
                documents = loader.load()
            else:
                logger.warning(f"⚠️ Không hỗ trợ định dạng file: {file_extension}")
                return []

            for doc in documents:
                doc.metadata.update(metadata)

            split_docs = self.text_splitter.split_documents(documents)
            logger.info(f"✅ Đã xử lý file {file_path}: {len(split_docs)} đoạn")
            return split_docs

        except Exception as e:
            logger.error(f"❌ Lỗi khi xử lý file {file_path}: {e}")

            return []

# import os
# import logging
# from typing import List, Optional
# from langchain.text_splitter import RecursiveCharacterTextSplitter
# from langchain.schema import Document
# from langchain_community.document_loaders import (
#     TextLoader,
#     PyPDFLoader,
#     Docx2txtLoader,
#     CSVLoader
# )
# from langchain_community.document_loaders import PyMuPDFLoader

# # OCR
# from pdf2image import convert_from_path
# import pytesseract
# import fitz  # PyMuPDF

# # Cấu hình Tesseract cho Windows
# pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
# os.environ["PATH"] += os.pathsep + r"C:\Users\DTC\Release-24.08.0-0\poppler-24.08.0\Library\bin"

# logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
# logger = logging.getLogger(__name__)

# class FileProcessor:
#     def __init__(self, chunk_size: int = 1000, chunk_overlap: int = 200):
#         self.chunk_size = chunk_size
#         self.chunk_overlap = chunk_overlap
#         self.text_splitter = RecursiveCharacterTextSplitter(
#             chunk_size=chunk_size,
#             chunk_overlap=chunk_overlap,
#             length_function=len
#         )

#     def extract_text_from_scanned_pdf(self, file_path: str) -> List[Document]:
#         """Trích xuất văn bản từ file scan bằng OCR với tối ưu cho tiếng Việt"""
#         logger.info("🔍 Đang thực hiện OCR file scan bằng Tesseract...")
#         try:
#             custom_config = r'--oem 3 --psm 6 -l vie+eng'  # Cấu hình tối ưu cho tiếng Việt và tiếng Anh
#             images = convert_from_path(file_path, dpi=300, grayscale=True, thread_count=4)
#             full_text = ""

#             for i, image in enumerate(images):
#                 image = image.convert('L')  # Chuyển ảnh xám để tăng hiệu suất OCR
#                 text = pytesseract.image_to_string(image, config=custom_config, timeout=30)
#                 logger.debug(f"📃 Trang {i + 1} OCR: {len(text)} ký tự")
#                 full_text += text + "\f"  # Phân tách trang bằng form feed

#             if not full_text.strip():
#                 logger.warning("⚠️ Không nhận diện được văn bản từ ảnh (OCR rỗng)")
#                 return []

#             full_text = self.post_process_ocr_text(full_text)  # Hậu xử lý văn bản OCR
#             doc = Document(page_content=full_text, metadata={"source": file_path, "ocr": True, "pages": len(images)})
#             return [doc]
#         except Exception as e:
#             logger.error(f"❌ Lỗi OCR với file {file_path}: {e}")
#             return []

#     def post_process_ocr_text(self, text: str) -> str:
#         """Hậu xử lý văn bản OCR"""
#         replacements = {
#             "v i": "vi", "q u a": "qua", "h o a": "hoa",
#             "đ i": "đi", "n g ư ờ i": "người"
#         }
#         for wrong, correct in replacements.items():
#             text = text.replace(wrong, correct)

#         # Loại bỏ ký tự xuống dòng thừa
#         text = text.replace("-\n", "").replace("\n", " ")
#         return text

#     def process_file(self, file_path: str, metadata: Optional[dict] = None) -> List[Document]:
#         if not os.path.exists(file_path):
#             logger.error(f"❌ File không tồn tại: {file_path}")
#             return []

#         try:
#             metadata = metadata or {}
#             metadata.update({"source": file_path, "extension": os.path.splitext(file_path)[1].lower()})
#             file_extension = metadata["extension"]
#             documents = []

#             # Hợp nhất quá trình xử lý tài liệu PDF, DOCX, CSV, TXT
#             if file_extension == '.pdf':
#                 documents = self.process_pdf(file_path)
#             elif file_extension in ['.docx', '.doc']:
#                 loader = Docx2txtLoader(file_path)
#                 documents = loader.load()
#             elif file_extension == '.csv':
#                 loader = CSVLoader(file_path)
#                 documents = loader.load()
#             elif file_extension in ['.txt', '.md']:
#                 loader = TextLoader(file_path, encoding='utf-8')
#                 documents = loader.load()
#             else:
#                 logger.warning(f"⚠️ Không hỗ trợ định dạng file: {file_extension}")
#                 return []

#             for doc in documents:
#                 doc.metadata.update(metadata)

#             split_docs = self.text_splitter.split_documents(documents)
#             logger.info(f"✅ Đã xử lý file {file_path}: {len(split_docs)} đoạn")
#             return split_docs

#         except Exception as e:
#             logger.error(f"❌ Lỗi khi xử lý file {file_path}: {e}")
#             return []

#     def process_pdf(self, file_path: str) -> List[Document]:
#         """Quy trình xử lý PDF và OCR nếu cần thiết"""
#         documents = []
#         if self.is_scanned_pdf(file_path):
#             logger.info("🔍 Phát hiện PDF scan → dùng OCR")
#             documents = self.extract_text_from_scanned_pdf(file_path)
#         else:
#             # Thử các loader cho PDF
#             loaders = [PyMuPDFLoader, PyPDFLoader]
#             for loader_class in loaders:
#                 try:
#                     loader = loader_class(file_path)
#                     documents = loader.load()
#                     if documents and any(doc.page_content.strip() for doc in documents):
#                         logger.info(f"✅ Thành công với {loader_class.__name__}")
#                         break
#                 except Exception as e:
#                     logger.warning(f"❌ {loader_class.__name__} lỗi: {e}")

#             # Nếu không có nội dung, sử dụng OCR
#             if not documents or all(len(doc.page_content.strip()) == 0 for doc in documents):
#                 documents = self.extract_text_from_scanned_pdf(file_path)
#         return documents

