import { useEffect, useState } from "react";
import {
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  LineChart,
  Line,
} from "recharts";
import { MonthlyTrendData } from "../../types";
import { getIncomeVsExpenseTrends } from "../../services/reportsService";

export default function MonthlyTrendChart() {
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
      <LineChart
      responsive
        data={data}
        margin={{
          top: 5,
          right: 0,
          left: 0,
          bottom: 5,
        }}
      >
        <CartesianGrid strokeDasharray="3 3" />
        {/*Each object in the array represents a bar
        dataKey prop maps each bar to the monthName*/}
        <XAxis dataKey="monthName" />
        <YAxis />
        <Tooltip formatter={(value: number) => [`$${value.toFixed(2)}`, '']} />
        <Legend />
        <Line type="monotone" dataKey="income" stroke="#10B981" name="Income" activeDot={{ r: 8 }}/>
        <Line type="monotone" dataKey="expenses" stroke="#EF4444" name="Expenses" />
      </LineChart>
    </ResponsiveContainer>
  );
}
