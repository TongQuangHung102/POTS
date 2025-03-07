from flask import Flask, request, jsonify
from openai_query import ask_openai, generate_large_mcq
import logging

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

def calculate_user_level(num_correct, total_questions, time_taken):
    """
    Tính toán mức độ người dùng dựa trên tỷ lệ câu trả lời đúng và thời gian làm bài.
    
    Args:
        num_correct (int): Số câu trả lời đúng
        total_questions (int): Tổng số câu hỏi
        time_taken (float): Thời gian làm bài (giây)
    
    Returns:
        int: Mức độ từ 1 đến 4
    """
    if total_questions <= 0:
        return 2  # Mặc định mức trung bình nếu không có dữ liệu
    
    accuracy = num_correct / total_questions
    
    # Điều chỉnh logic tính toán mức độ chi tiết hơn
    if accuracy >= 0.8 and time_taken < 300:
        return 4  # Mức độ cao
    elif accuracy >= 0.7:
        return 3  # Mức độ trung bình cao
    elif accuracy >= 0.5:
        return 2  # Mức độ trung bình
    else:
        return 1  # Mức độ thấp

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
        
        # Xác định mức độ người dùng
        try:
            num_correct = int(results.get('num_correct', 0))
            total_questions = int(results.get('total_questions', 0))
            time_taken = float(results.get('time_taken', 0))
            
            user_level = calculate_user_level(num_correct, total_questions, time_taken)
        except (ValueError, TypeError):
            user_level = 2  # Mức độ trung bình mặc định
        
        # Sinh câu hỏi với mức độ tương ứng
        questions = ask_openai(
            {
                'context': user_question, 
                'level_id': user_level
            }, 
            num_questions, 
            difficulty_mode='similar'
        )
        
        return jsonify({"questions": questions})
    
    except Exception as e:
        # logger.error(f"Lỗi trong generate-question: {e}", exc_info=True)
        return jsonify({"error": "Đã xảy ra lỗi không mong muốn"}), 500

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