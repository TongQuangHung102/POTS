import { useNavigate } from "react-router-dom";

const BackLink = () => {
  const navigate = useNavigate();

  return <span style={{ cursor: "pointer", color: "blue" }} onClick={() => navigate(-1)}>Quay lại</span>;
};

export default BackLink;
