from flask import Flask, request, jsonify
from openai_query import ask_openai, generate_large_mcq
import logging

# from vector_db import search_in_vector_db

app = Flask(__name__)

# # Cấu hình logging để ghi lại các lỗi
# logging.basicConfig(level=logging.INFO, 
#                     format='%(asctime)s - %(levelname)s - %(message)s')
# logger = logging.getLogger(__name__)

def validate_input(user_question, num_questions):
    """
    Kiểm tra tính hợp lệ của đầu vào.
    
    Args:
        user_question (str): Chủ đề hoặc câu hỏi mẫu của người dùng
        num_questions (int): Số lượng câu hỏi cần sinh
    
    Returns:
        tuple: (is_valid, error_message)
    """
    if not user_question:
        return False, "Không có câu hỏi được cung cấp"
    
    if not isinstance(num_questions, int) or num_questions <= 0:
        return False, "Số lượng câu hỏi không hợp lệ"
    
    if num_questions > 50:
        return False, "Số lượng câu hỏi không được vượt quá 50"
    
    return True, None

@app.route('/generate-mcq', methods=['POST'])
def generate_mcq():
    """API để tạo câu hỏi trắc nghiệm ngẫu nhiên từ OpenAI."""
    try:
        # Hỗ trợ cả JSON và form data
        data = request.get_json() if request.is_json else request.form
        
        user_question = data.get('question', '').strip()
        num_questions = int(data.get('num_questions', 5))
        
        # Kiểm tra đầu vào
        is_valid, error_message = validate_input(user_question, num_questions)
        if not is_valid:
            return jsonify({"error": error_message}), 400
        
        # Sinh câu hỏi
        if num_questions > 20:
            questions = generate_large_mcq(user_question, num_questions)
        else:
            questions = ask_openai(user_question, num_questions)
        
        return jsonify({"questions": questions})
    
    except Exception as e:
        # logger.error(f"Lỗi trong generate-mcq: {e}", exc_info=True)
        return jsonify({"error": "Đã xảy ra lỗi không mong muốn"}), 500

@app.route('/generate-question', methods=['POST'])
def generate_question():
    """
    API để tạo câu hỏi trắc nghiệm từ OpenAI 
    dựa trên kết quả bài kiểm tra trước đó của người dùng.
    """
    try:
        # Hỗ trợ cả JSON và form data
        data = request.get_json() if request.is_json else request.form
        
        user_question = data.get('question', '').strip()
        num_questions = int(data.get('num_questions', 5))
        results = data.get('results', {})
        
        # Kiểm tra đầu vào
        is_valid, error_message = validate_input(user_question, num_questions)
        if not is_valid:
            return jsonify({"error": error_message}), 400
        
        # Lấy thông tin từ kết quả người dùng
        try:
            num_correct = int(results.get('num_correct', 0))
            total_questions = int(results.get('total_questions', 0))
            time_taken = float(results.get('time_taken', 0))
        except (ValueError, TypeError):
            return jsonify({"error": "Dữ liệu kết quả không hợp lệ"}), 400
        
        # Tạo câu hỏi với mức độ tương ứng từ OpenAI
        questions = ask_openai(
            {
                'context': user_question, 
                'results': results  # Truyền kết quả bài làm vào để AI tự phân tích
            }, 
            num_questions, 
            difficulty_mode='similar'
        )
        
        return jsonify({"questions": questions})
    
    except Exception as e:
        return jsonify({"error": "Đã xảy ra lỗi không mong muốn"}), 500


# @app.route('/generate-question-from-db', methods=['POST'])
# def generate_question_from_db():
#     """API để tạo câu hỏi từ OpenAI với kiến thức trong Vector DB."""
#     try:
#         data = request.get_json()
#         chapter = data.get('chapter', '').strip()
#         num_questions = int(data.get('num_questions', 5))
        
#         # Tìm kiếm nội dung liên quan trong Vector DB
#         context = search_in_vector_db(chapter)
        
#         # Kiểm tra nếu không tìm thấy dữ liệu
#         if not context:
#             return jsonify({"error": "Không tìm thấy kiến thức liên quan"}), 404
        
#         # Sinh câu hỏi từ OpenAI với context tìm được
#         questions = ask_openai(context, num_questions)
        
#         return jsonify({"questions": questions})
    
#     except Exception as e:
#         return jsonify({"error": "Đã xảy ra lỗi không mong muốn"}), 500


@app.route('/')
def home():
    """Trang chủ API"""
    return jsonify({
        "message": "Chào mừng đến với API Sinh Câu Hỏi",
        "endpoints": [
            "/generate-mcq",
            "/generate-question"
        ]
    })

# Cấu hình CORS và bảo mật (khuyến nghị sử dụng flask-cors)
@app.after_request
def add_security_headers(response):
    """Thêm các header bảo mật"""
    response.headers['X-Content-Type-Options'] = 'nosniff'
    response.headers['X-Frame-Options'] = 'SAMEORIGIN'
    return response

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)