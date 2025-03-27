import React, { useEffect, useState } from "react";
import styles from "./HomePage.module.css";
import { FaFacebook, FaTwitter, FaInstagram, FaLinkedin, FaGoogle, FaTableCellsLarge, FaLayerGroup, FaBriefcase, FaComments, FaLocationDot, FaEnvelope, FaPhone } from "react-icons/fa6";

const HomePage = () => {
  const [text, setText] = useState("");
  const fullText = "Luyện tập trắc nghiệm";
  let index = 1;
  let isDeleting = false;

  useEffect(() => {
    const typeEffect = () => {
      if (!isDeleting && index <= fullText.length) {
        setText(fullText.substring(0, index));
        index++;
      } else if (isDeleting && index >= 0) {
        setText(fullText.substring(0, index));
        index--;
      }
      let speed = isDeleting ? 80 : 150;
      if (index === fullText.length + 1) {
        isDeleting = true;
        speed = 1500;
      } else if (index === 1) {
        isDeleting = false;
        speed = 500;
      }
      setTimeout(typeEffect, speed);
    };
    typeEffect();
  }, []);

  useEffect(() => {
    const title = document.querySelector("." + styles.bodyTitle);
    const item = document.querySelector("." + styles.bodyItems);

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            title.classList.add(styles.show);
            item.classList.add(styles.show);
          }
        });
      },
      { threshold: 0.5 }
    );

    if (title) observer.observe(title);
    if (item) observer.observe(item);
  }, []);

  return (
    <div>
      <header className={styles.menu}>
        <div className={styles.logo}></div>
        <nav className={styles.menuList}>
          <p>Đăng nhập</p>
        </nav>
      </header>

      <section className={styles.background}>
        <div className={styles.backgroundContent}>
          <h2 className={styles.gradientText}>{text}</h2>
          <h3>Ứng dụng AI tự động điều chỉnh câu hỏi theo năng lực của từng học sinh.</h3>
          <div className={styles.logoIcon}>
            <FaFacebook /> <FaTwitter /> <FaInstagram /> <FaLinkedin /> <FaGoogle />
          </div>
          <button>Bắt đầu ngay</button>
        </div>
      </section>

      <section className={styles.body}>
        <div className={styles.bodyTitle}>
          <h3>Các tính năng nổi bật</h3>
        </div>
        <div className={styles.bodyItems}>
          {features.map((feature, index) => (
            <div className={styles.items} key={index}>
              {feature.icon}
              <h4>{feature.title}</h4>
              <p>{feature.description}</p>
            </div>
          ))}
        </div>
      </section>

      <section className={styles.body2}>
        <h2>Giao diện thân thiện dễ dàng sử dụng</h2>
        <div className={styles.body2Img}>
          <img src="./images/work3.jpg" alt="Giao diện 1" />
          <img src="./images/work4.jpg" alt="Giao diện 2" />
          <img src="./images/work5.jpg" alt="Giao diện 3" />
        </div>
      </section>

      <section className={styles.body3}>
        <h1>Contact</h1>
        <div className={styles.bodyContact}>
          <div className={styles.contactForm}>
            <h3>CONTACT FORM</h3>
            <input type="text" placeholder="Name" />
            <input type="text" placeholder="Email" />
            <input type="text" placeholder="Subject" />
            <input type="text" placeholder="Your Message" />
            <button>SEND MESSAGE</button>
          </div>
          <div className={styles.contactAddress}>
            <h3>CONTACT ADDRESS</h3>
            <div><FaLocationDot /> <p>Ha Noi, Viet Nam</p></div>
            <div><FaEnvelope /> <p>Mail@mail.com</p></div>
            <div><FaPhone /> <p>0123456789</p></div>
          </div>
        </div>
      </section>

      <footer className={styles.footer}>
        <FaFacebook /> <FaTwitter /> <FaInstagram /> <FaLinkedin /> <FaGoogle />
      </footer>
    </div>
  );
};

const features = [
  { icon: <FaTableCellsLarge />, title: "Luyện tập cá nhân hóa", description: "Đề xuất bài tập dựa trên năng lực của học sinh" },
  { icon: <FaLayerGroup />, title: "Phân tích kết quả", description: "Báo cáo chi tiết giúp học sinh và phụ huynh theo dõi tiến độ" },
  { icon: <FaBriefcase />, title: "Thi thử & Đánh giá", description: "Tổ chức các bài thi online theo từng chuyên đề" },
  { icon: <FaComments />, title: "Ứng dụng AI phân tích kết quả", description: "Sau mỗi lần luyện tập, kết quả sẽ được AI đánh giá và đề xuất câu hỏi phù hợp" },
];

export default HomePage;
