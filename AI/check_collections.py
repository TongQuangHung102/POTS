import logging

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

def list_all_collections():
    from chromadb import PersistentClient
    client = PersistentClient(path="./chroma_db")
    collections = client.list_collections()
    print("Danh sách collections:")
    for c in collections:
        print(f"- {c.name}")
    return collections  # Trả về collections để sử dụng ở hàm khác

def collection_exists(name: str) -> bool:
    """Kiểm tra collection có tồn tại không"""
    try:
        # Liệt kê tất cả collection
        from chromadb import PersistentClient
        client = PersistentClient(path="./chroma_db")
        collections = client.list_collections()
        # So sánh không phân biệt chữ hoa/thường
        result = any(c.name.lower() == name.lower() for c in collections)
        print(f"Collection '{name}' tồn tại: {result}")
        return result
    except Exception as e:
        logging.error(f"Lỗi khi kiểm tra collection: {str(e)}")
        return False

if __name__ == "__main__":
    # Gọi hàm khi chạy trực tiếp file này
    collections = list_all_collections()
    
    # Kiểm tra một số tên collections
    collection_exists("DiaLy1")  # Kiểm tra với L viết hoa
    collection_exists("Dialy1")  # Kiểm tra với l viết thường
    
    # In ra tên của tất cả collection hiện có để so sánh
    print("\nTên chính xác của các collection (không chuyển đổi):")
    for c in collections:
        print(f"- '{c.name}'")