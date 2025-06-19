FROM python:3.10-slim

# Set working directory
WORKDIR /app

# Copy requirements.txt vào container
COPY requirements.txt ./ 

# Cài đặt các dependencies
RUN pip install --no-cache-dir -r requirements.txt

# Sao chép mã nguồn vào container
COPY . .

# Sao chép các collection và dữ liệu vector DB vào container
# Giả sử các collection của bạn nằm trong thư mục "chroma_db" trên máy chủ thực
COPY ./chroma_db /app/chroma_db

# Expose port (giả sử AI service chạy trên port 5000)
EXPOSE 5000

# Command để chạy ứng dụng AI (giả sử là một Flask app)
CMD ["python", "app.py"]