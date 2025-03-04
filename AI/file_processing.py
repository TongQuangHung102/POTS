import chardet
from langchain_community.document_loaders import PyPDFLoader

from docx import Document

def detect_encoding(file):
    """Phát hiện mã hóa của file văn bản."""
    raw_data = file.read(10000)
    result = chardet.detect(raw_data)
    file.seek(0)
    return result['encoding']

def read_txt(file):
    """Đọc nội dung từ file TXT"""
    encoding = detect_encoding(file)
    return file.read().decode(encoding, errors='ignore')

def read_pdf(file_path):
    """Đọc nội dung từ file PDF"""
    loader = PyPDFLoader(file_path)
    pages = loader.load()
    return "\n".join([page.page_content for page in pages])

def read_docx(file_path):
    """Đọc nội dung từ file DOCX"""
    doc = Document(file_path)
    return "\n".join([para.text for para in doc.paragraphs])

def extract_text(file_path, file_type):
    """Trích xuất nội dung từ file dựa vào loại file."""
    if file_type == "pdf":
        return read_pdf(file_path)
    elif file_type == "docx":
        return read_docx(file_path)
    elif file_type == "txt":
        with open(file_path, "rb") as f:
            return read_txt(f)
    else:
        raise ValueError("Unsupported file type")
