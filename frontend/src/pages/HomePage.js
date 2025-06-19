import React, { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

// Giả lập dữ liệu
const featuresData = [
  {
    id: 1,
    title: "Cá nhân hóa nội dung học tập",
    description: "Hệ thống trí tuệ nhân tạo phân tích điểm mạnh, điểm yếu để tạo lộ trình học tập phù hợp riêng cho bạn.",
    icon: "📊"
  },
  {
    id: 2,
    title: "Ngân hàng câu hỏi đa dạng",
    description: "Hơn 50,000 câu hỏi trắc nghiệm trong nhiều lĩnh vực từ cơ bản đến nâng cao.",
    icon: "📚"
  },
  {
    id: 3,
    title: "Phân tích kết quả chi tiết",
    description: "Báo cáo chi tiết về hiệu suất học tập, giúp bạn theo dõi tiến độ và cải thiện điểm số.",
    icon: "📈"
  },
  {
    id: 4,
    title: "Theo dõi kết quả học của con",
    description: "Phụ huynh có thể theo dõi kết quả học tập của con một các dễ dàng",
    icon: "📱"
  }
];

const testimonials = [
  {
    id: 1,
    name: "Tống Quang Việt Hưng",
    role: "Học sinh lớp 12",
    content: "Nhờ hệ thống cá nhân hóa, em đã cải thiện điểm số của mình từ 7 lên 9.5 chỉ trong 3 tháng!",
    avatar: "/images/user1.jpg"
  },
  {
    id: 2,
    name: "Tống Văn Vinh",
    role: "Sinh viên đại học",
    content: "Platform này giúp mình ôn thi hiệu quả hơn rất nhiều. Các đề trắc nghiệm đúng trọng tâm, tiết kiệm thời gian ôn luyện.",
    avatar: "/images/user2.jpg"
  },
  {
    id: 3,
    name: "Trần Thị Thúy",
    role: "Học sinh cấp 2",
    content: "Tôi thường giới thiệu bạn bè của mình sử dụng nền tảng này. Hệ thống cá nhân hóa thực sự hiệu quả!",
    avatar: "/images/hsnu2.jpg"
  }
];

export default function Homepage() {
  const [isScrolled, setIsScrolled] = useState(false);
  const [stats, setStats] = useState({ users: 0, questions: 0, parent: 0 });

  const navigate = useNavigate();
  // Hiệu ứng đếm số
  useEffect(() => {
    const interval = setInterval(() => {
      setStats(prev => {
        return {
          users: prev.users >= 1000 ? 1000 : prev.users + 50,
          questions: prev.questions >= 10000 ? 10000 : prev.questions + 500,
          parent: prev.parent >= 800 ? 800 : prev.parent + 40
        };
      });
    }, 50);

    return () => clearInterval(interval);
  }, []);

  // Hiệu ứng navbar khi cuộn
  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > 50) {
        setIsScrolled(true);
      } else {
        setIsScrolled(false);
      }
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <div className="quiz-app">
      {/* Navbar */}

      <nav className={`navbar navbar-expand-lg navbar-light fixed-top ${isScrolled ? "bg-white shadow-sm" : "bg-transparent"}`}>
        <div className="container">
          <Link className="navbar-brand fw-bold text-primary" to="/">MetaSchool</Link> 
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav ms-auto">
              <li className="nav-item">
                <Link className="nav-link" to="/">Trang chủ</Link> {/* Sử dụng Link */}
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/practice">Luyện tập</Link> {/* Sử dụng Link */}
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/about">Giới thiệu</Link> {/* Sử dụng Link */}
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/contact">Liên hệ</Link> {/* Sử dụng Link */}
              </li>
            </ul>
            <div className="ms-lg-3">
              <Link to="/login" className="btn btn-outline-primary me-2">Đăng nhập</Link> {/* Sử dụng Link */}
              <Link to="/register" className="btn btn-primary">Đăng ký</Link> {/* Sử dụng Link */}
            </div>
          </div>
        </div>
      </nav>


      {/* Hero Section */}
      <section className="py-5 bg-light">
        <div className="container py-5 mt-5">
          <div className="row align-items-center">
            <div className="col-lg-6 mb-5 mb-lg-0">
              <h1 className="display-4 fw-bold mb-3">Nâng cao kiến thức với trắc nghiệm <span className="text-primary">cá nhân hóa</span></h1>
              <p className="lead text-secondary mb-4">
                Luyện tập với hệ thống trắc nghiệm thông minh giúp bạn học hiệu quả hơn
                với việc áp dụng AI để cá nhân hóa riêng bài tập cho từng cá nhân.
              </p>
              <div className="d-flex flex-wrap gap-2">
                <a className="btn btn-primary btn-lg">Bắt đầu miễn phí</a>
              </div>
              <div className="mt-4 d-flex align-items-center">
                <div className="d-flex">
                  
                </div>
                <p className="mb-0 ms-3 text-secondary">
                  Được tin dùng bởi <span className="fw-bold">{stats.users.toLocaleString()}</span> học viên
                </p>
              </div>
            </div>
            <div className="col-lg-6">
              <div className="position-relative">
                <div className="bg-primary rounded-4 p-3 shadow">
                  <img src="/images/bg3.jpg" className="img-fluid rounded-3" alt="Student studying" />
                </div>
                <div className="position-absolute top-100 start-0 translate-middle">
                  <div className="bg-white rounded-pill p-3 shadow d-flex align-items-center">


                  </div>
                </div>
                <div className="position-absolute top-0 end-0 translate-middle">
                  <div className="bg-white rounded-pill p-3 shadow">
                    <p className="mb-0 fw-bold">Trải nghiệm học tập thông minh</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="py-5 bg-white">
        <div className="container py-4">
          <div className="row g-4 text-center">
            <div className="col-md-4">
              <div className="p-4 border rounded-4 shadow-sm h-100">
                <div className="display-4 fw-bold text-primary mb-2">{stats.users.toLocaleString()}+</div>
                <p className="lead mb-0">Học viên đang sử dụng</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="p-4 border rounded-4 shadow-sm h-100">
                <div className="display-4 fw-bold text-primary mb-2">{stats.parent.toLocaleString()}+</div>
                <p className="lead mb-0">Phụ huynh đang sử dụng</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="p-4 border rounded-4 shadow-sm h-100">
                <div className="display-4 fw-bold text-primary mb-2">{stats.questions}+</div>
                <p className="lead mb-0">Câu hỏi trắc nghiệm</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-5 bg-light">
        <div className="container py-5">
          <div className="text-center mb-5">
            <h2 className="display-5 fw-bold">Tính năng nổi bật</h2>
            <p className="lead text-secondary">Những công nghệ giúp bạn học hiệu quả hơn</p>
          </div>
          <div className="row g-4">
            {featuresData.map(feature => (
              <div key={feature.id} className="col-md-6 col-lg-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body p-4">
                    <div className="display-5 mb-3">{feature.icon}</div>
                    <h5 className="card-title fw-bold">{feature.title}</h5>
                    <p className="card-text text-secondary">{feature.description}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* How It Works */}
      <section className="py-5 bg-white">
        <div className="container py-5">
          <div className="text-center mb-5">
            <h2 className="display-5 fw-bold">Cách thức hoạt động</h2>
            <p className="lead text-secondary">Quy trình đơn giản, hiệu quả cao</p>
          </div>
          <div className="row g-4">
            <div className="col-md-4">
              <div className="text-center p-4">
                <div className="bg-light rounded-circle mx-auto d-flex align-items-center justify-content-center mb-4" style={{ width: "100px", height: "100px" }}>
                  <span className="display-6">1</span>
                </div>
                <h4 className="fw-bold">Tạo tài khoản</h4>
                <p className="text-secondary">Đăng ký nhanh chóng chỉ với email hoặc tài khoản mạng xã hội</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="text-center p-4">
                <div className="bg-light rounded-circle mx-auto d-flex align-items-center justify-content-center mb-4" style={{ width: "100px", height: "100px" }}>
                  <span className="display-6">2</span>
                </div>
                <h4 className="fw-bold">Kiểm tra đầu vào</h4>
                <p className="text-secondary">Làm bài kiểm tra để hệ thống hiểu rõ trình độ và nhu cầu của bạn</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="text-center p-4">
                <div className="bg-light rounded-circle mx-auto d-flex align-items-center justify-content-center mb-4" style={{ width: "100px", height: "100px" }}>
                  <span className="display-6">3</span>
                </div>
                <h4 className="fw-bold">Bắt đầu học tập</h4>
                <p className="text-secondary">Các bài tập đã được cá nhân hóa theo trình độ của bạn và hãy cải thiện kết quả của bạn</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Testimonials */}
      <section className="py-5 bg-light">
        <div className="container py-5">
          <div className="text-center mb-5">
            <h2 className="display-5 fw-bold">Người dùng nói gì?</h2>
            <p className="lead text-secondary">Những đánh giá từ học viên đã sử dụng nền tảng</p>
          </div>
          <div className="row g-4">
            {testimonials.map(item => (
              <div key={item.id} className="col-md-4">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body p-4">
                    <div className="d-flex mb-3">
                      <img style={{height:'50px'}} src={item.avatar} className="rounded-circle me-3" alt={item.name} />
                      <div>
                        <h5 className="card-title mb-0 fw-bold">{item.name}</h5>
                        <p className="text-secondary small">{item.role}</p>
                      </div>
                    </div>
                    <p className="card-text fst-italic">"{item.content}"</p>
                    <div className="text-warning">
                      {"★".repeat(5)}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-5 bg-primary text-white">
        <div className="container py-5">
          <div className="row align-items-center">
            <div className="col-lg-8 mb-4 mb-lg-0">
              <h2 className="display-5 fw-bold">Sẵn sàng cải thiện kết quả học tập?</h2>
              <p className="lead mb-0">Đăng ký ngay hôm nay để nhận 7 ngày dùng thử miễn phí tất cả tính năng!</p>
            </div>
            <div className="col-lg-4 text-lg-end">
              <Link to="/register" className="btn btn-success btn-lg">Bắt đầu ngay</Link> {/* Sử dụng Link */}
            </div>
          </div>
        </div>
      </section>


      {/* Footer */}
      <footer className="py-5 bg-dark text-white">
        <div className="container">
          <div className="row g-4">
            <div className="col-lg-4">
              <h4 className="fw-bold mb-4">QuizViet</h4>
              <p>Nền tảng học tập trắc nghiệm cá nhân hóa hàng đầu Việt Nam, giúp học sinh, sinh viên và người đi làm nâng cao kiến thức một cách hiệu quả nhất.</p>
              <div className="d-flex gap-3 mt-4">
                <Link to="https://www.facebook.com" className="text-white">
                  <i className="bi bi-facebook"></i>
                </Link>
                <Link to="https://twitter.com" className="text-white">
                  <i className="bi bi-twitter"></i>
                </Link>
                <Link to="https://www.instagram.com" className="text-white">
                  <i className="bi bi-instagram"></i>
                </Link>
                <Link to="https://www.youtube.com" className="text-white">
                  <i className="bi bi-youtube"></i>
                </Link>
              </div>
            </div>
            <div className="col-6 col-lg-2">
              <h5 className="fw-bold mb-4">Liên kết</h5>
              <ul className="list-unstyled">
                <li className="mb-2"><Link to="/" className="text-decoration-none text-secondary">Trang chủ</Link></li>
                <li className="mb-2"><Link to="/exams" className="text-decoration-none text-secondary">Đề thi</Link></li>
                <li className="mb-2"><Link to="/courses" className="text-decoration-none text-secondary">Khóa học</Link></li>
                <li className="mb-2"><Link to="/about" className="text-decoration-none text-secondary">Giới thiệu</Link></li>
                <li className="mb-2"><Link to="/contact" className="text-decoration-none text-secondary">Liên hệ</Link></li>
              </ul>
            </div>
            <div className="col-6 col-lg-2">
              <h5 className="fw-bold mb-4">Tài nguyên</h5>
              <ul className="list-unstyled">
                <li className="mb-2"><Link to="/blog" className="text-decoration-none text-secondary">Blog</Link></li>
                <li className="mb-2"><Link to="/guides" className="text-decoration-none text-secondary">Hướng dẫn</Link></li>
                <li className="mb-2"><Link to="/faq" className="text-decoration-none text-secondary">FAQ</Link></li>
                <li className="mb-2"><Link to="/resources" className="text-decoration-none text-secondary">Tài liệu học tập</Link></li>
                <li className="mb-2"><Link to="/forum" className="text-decoration-none text-secondary">Diễn đàn</Link></li>
              </ul>
            </div>
          </div>
        </div>
      </footer>

    </div>
  );
}