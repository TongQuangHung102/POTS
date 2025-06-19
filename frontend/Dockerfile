# Build React app
FROM node:16 AS build

# Set working directory
WORKDIR /app

# Copy package.json và package-lock.json
COPY package*.json ./

# Cài đặt dependencies
RUN npm install

# Sao chép mã nguồn vào container
COPY . .

# Build ứng dụng React
RUN npm run build

# Chạy ứng dụng React bằng nginx
FROM nginx:alpine

# Copy các file build vào thư mục của nginx để phục vụ
COPY --from=build /app/build /usr/share/nginx/html

# Cấu hình nginx (copy file cấu hình của nginx vào container)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose cổng 80 (nginx chạy trên cổng 80 nội bộ)
EXPOSE 80

# Start nginx server
CMD ["nginx", "-g", "daemon off;"]
