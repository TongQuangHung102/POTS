import { useState, useEffect } from "react";

const MonthSelector = ({ onMonthChange, selectedMonth }) => {
  const today = new Date();
  const currentMonth = today.getMonth() + 1;
  const currentYear = today.getFullYear();

  // Tạo danh sách 6 tháng gần nhất
  const months = [];
  for (let i = 0; i < 6; i++) {
    const date = new Date(currentYear, currentMonth - 1 - i, 1);
    const month = date.getMonth() + 1;
    const year = date.getFullYear();
    months.push({ value: `${year}-${month}`, label: `Tháng ${month}/${year}` });
  }

  const handleChange = (e) => {
    if (onMonthChange) {
      onMonthChange(e.target.value);
    }
  };

  return (
    <div>
      <select className="stats-select" value={selectedMonth} onChange={handleChange}>
        {months.map((m) => (
          <option key={m.value} value={m.value}>
            {m.label}
          </option>
        ))}
      </select>
    </div>
  );
};

export default MonthSelector;
