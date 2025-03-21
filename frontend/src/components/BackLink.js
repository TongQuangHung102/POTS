import { useNavigate } from "react-router-dom";

const BackLink = () => {
  const navigate = useNavigate();

  return <div style={{ cursor: "pointer", color: "blue", marginBottom: "15px" }} onClick={() => navigate(-1)}>Quay lại</div>;
};

export default BackLink;
