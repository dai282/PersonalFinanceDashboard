import { useEffect, useState } from "react";
import api from "../../services/api";
import { MonthlySummary } from "../../types";

interface SummaryCardProps {
  type: string;
}

export default function SummaryCard({ type }: SummaryCardProps) {
  const [monthlySummary, setMonthlySummary] = useState<MonthlySummary>({
    month: 1,
    year: 2025,
    totalIncome: 0,
    totalExpenses: 0,
    balance: 0,
    transactionCount: 0,
    savingsRate: 0,
  });

  useEffect(() => {
    const getMonthlySummary = async () => {
      try {
        const response = await api.get<MonthlySummary>(
          `/Reports/monthly-summary`
        );
        const summary = response.data;
        setMonthlySummary({
          month: summary.month,
          year: summary.year,
          totalIncome: summary.totalIncome,
          totalExpenses: summary.totalExpenses,
          balance: summary.balance,
          transactionCount: summary.transactionCount,
          savingsRate: summary.savingsRate,
        });
      } catch (error) {
        console.error(error);
      }
    };
    getMonthlySummary();
  }, []);

  return (
    <>
      <div className="bg-white rounded-lg shadow p-6">
        <p className="text-gray-500 text-sm font-medium">Total {type}</p>
        <p
          className={`text-3xl font-bold ${
            type === "Income"
              ? "text-green-600"
              : type === "Expenses"
              ? "text-red-600"
              : "text-blue-600"
          } mt-2`}
        >
          $
          {type === "Income"
            ? monthlySummary.totalIncome
            : type === "Expenses"
            ? monthlySummary.totalExpenses
            : monthlySummary.balance}
        </p>
      </div>
    </>
  );
}
