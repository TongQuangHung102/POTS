import React from 'react';
import styles from './Loading.module.css';  // Đảm bảo bạn đã tạo file CSS riêng cho Loading

const Loading = ({ message = "Đang tải...", color = "#3498db" }) => {
    return (
      <div className={styles.loading}>
        <div className={styles.spinner} style={{ borderTopColor: color }}></div>
        <p>{message}</p>
      </div>
    );
  }
export default Loading;