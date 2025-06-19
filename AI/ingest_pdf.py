
import logging
import os
from file_processing import FileProcessor
from vector_db import VectorDB

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# ✅ Cấu hình
OPENAI_API_KEY = "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"  # 🔒 Thay bằng API key thật của bạn
FILE_PATH = "uploads/Toán lớp 5 tập 1 KNTT.pdf"
COLLECTION_NAME = "Toan5"  # 📁 Tên collection trong vector store

def main():
    # Kiểm tra file tồn tại
    if not os.path.exists(FILE_PATH):
        logger.error(f"❌ File không tồn tại: {FILE_PATH}")
        return

    # 1. Xử lý file thành danh sách Document
    processor = FileProcessor()
    documents = processor.process_file(FILE_PATH)

    if not documents:
        logger.warning("⚠️ Không có đoạn văn bản nào được tạo.")
        return

    logger.info(f"✅ Đã xử lý thành công! Tổng cộng {len(documents)} đoạn.")

    # 2. Khởi tạo vector store và lưu documents
    vector_db = VectorDB(openai_api_key=OPENAI_API_KEY)
    try:
        vector_db.add_documents(documents, collection_name=COLLECTION_NAME)
        logger.info(f"📦 Đã lưu embeddings vào collection: '{COLLECTION_NAME}'")
    except Exception as e:
        logger.error(f"❌ Lỗi khi lưu vào vector store: {e}")


if __name__ == "__main__":
    main()
