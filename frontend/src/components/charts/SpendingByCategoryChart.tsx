import React, { useEffect, useState } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { SpendingByCategoryData } from "../../types";
import { getSpendingByCategory } from "../../services/reportsService";

const COLORS = [
  "#0088FE",
  "#00C49F",
  "#FFBB28",
  "#FF8042",
  "#8884D8",
  "#FF6B6B",
  "#4ECDC4",
  "#45B7D1",
  "#F7DC6F",
];

export default function SpendingByCategoryChart({
  isAnimationActive = true,
}: {
  isAnimationActive?: boolean;
}) {
  const [data, setData] = useState<SpendingByCategoryData[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getReport = async () => {
      try {
        const report = await getSpendingByCategory();
        setData(report.categories);
      } catch (error) {
        console.error("Failed to fetch spending data:", error);
      } finally {
        setLoading(false);
      }
    };
    getReport();
  }, []);

  const RADIAN = Math.PI / 180;
  const renderCustomizedLabel = ({
    cx,
    cy,
    midAngle,
    innerRadius,
    outerRadius,
    percent,
    name,
  }: any) => {
    if (percent < 0.05) return null;
    const radius = innerRadius + (outerRadius - innerRadius) * 0.5;
    const x = cx + radius * Math.cos(-midAngle * RADIAN);
    const y = cy + radius * Math.sin(-midAngle * RADIAN);

    return (
      <text
        x={x}
        y={y}
        fill="white"
        textAnchor="middle"
        dominantBaseline="central"
        fontSize={14}
      >
        {`${name}: ${(percent * 100).toFixed(1)}%`}
      </text>
    );
  };

  if (loading) {
    return (
      <div className="h-64 flex items-center justify-center">
        <div className="text-gray-500">Loading chart...</div>
      </div>
    );
  }

  return (
    <ResponsiveContainer width="100%" height={400}>
      <PieChart>
        <Tooltip />
        <Pie
          data={data as any}
          cx="50%"
          cy="50%"
          labelLine={false}
          label={renderCustomizedLabel}
          fill="#8884d8"
          dataKey="amount"
          nameKey="categoryName"
          isAnimationActive={isAnimationActive}
        >
          {data.map((entry, index) => (
            <Cell
              key={`cell-${entry.categoryName}`}
              fill={COLORS[index % COLORS.length]}
            />
          ))}
        </Pie>
      </PieChart>
    </ResponsiveContainer>
  );
}
