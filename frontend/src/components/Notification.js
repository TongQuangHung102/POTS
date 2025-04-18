
import styles from "./NotificationDropdown.module.css"; 
import { formatDateTime } from "../utils/timeUtils";

const NotificationDropdown = ({ notifications }) => {

  return (
    <div className={styles.notificationDropdown}>
      <h3>Thông báo</h3>
      <ul>
        {notifications.length > 0 ? (
          notifications.map((noti) => (
            <li
              key={noti.id}
              className={noti.isRead ? styles.read : styles.unread}
            >
              <strong>{noti.title}:</strong> {noti.content} <p>{formatDateTime(noti.date)}</p>
            </li>
          ))
        ) : (
          <li>Không có thông báo mới</li>
        )}
      </ul>
    </div>
  );
};

export default NotificationDropdown;
