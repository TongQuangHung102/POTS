import { useState, useEffect } from "react";

const useManagedGrades = () => {
  const [managedGrades, setManagedGrades] = useState(() => {
    const savedGrades = localStorage.getItem("managedGrades");
    return savedGrades ? JSON.parse(savedGrades) : [];
  });

  const [selectedGrade, setSelectedGrade] = useState(() => {
    return managedGrades.length > 0 ? managedGrades[0] : null;
  });

  const [isData, setIsData] = useState(true);

  useEffect(() => {
    localStorage.setItem("managedGrades", JSON.stringify(managedGrades));

    if (managedGrades.length > 0 && !selectedGrade) {
      setSelectedGrade(managedGrades[0]);
    }

    if (managedGrades.length === 0) {
      setIsData(false);
    }
  }, [managedGrades]);

  return { managedGrades, setManagedGrades, selectedGrade, setSelectedGrade, isData };
};

export default useManagedGrades;
