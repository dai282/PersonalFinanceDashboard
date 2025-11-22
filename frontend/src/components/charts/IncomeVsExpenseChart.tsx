import { useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import { MonthlyTrendData } from "../../types";
import { getIncomeVsExpenseTrends } from "../../services/reportsService";

export default function IncomeVsExpenseChart() {
  const [data, setData] = useState<MonthlyTrendData[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getReport = async () => {
      try {
        const report = await getIncomeVsExpenseTrends();
        setData(report.trends);
      } catch (error) {
        console.error("Failed to fetch spending data:", error);
      } finally {
        setLoading(false);
      }
    };
    getReport();
  }, []);

  if (loading) {
    return (
      <div className="h-64 flex items-center justify-center">
        <div className="text-gray-500">Loading chart...</div>
      </div>
    );
  }

  return (
    <ResponsiveContainer width="100%" height={400}>
      {/*Passing in an array of objects, each object is spending info for 1 month*/}
      <BarChart
        data={data}
        margin={{
          top: 5,
          right: 30,
          left: 20,
          bottom: 5,
        }}
      >
        <CartesianGrid strokeDasharray="3 3" />
        {/*Each object in the array represents a bar*/}
        <XAxis dataKey="monthName" />
        <YAxis />
        <Tooltip formatter={(value: number) => [`$${value.toFixed(2)}`, '']} />
        <Legend />
        <Bar dataKey="income" fill="#10B981" name="Income" />
        <Bar dataKey="expenses" fill="#EF4444" name="Expenses" />
      </BarChart>
    </ResponsiveContainer>
  );
}
