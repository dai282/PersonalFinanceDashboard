import React from "react";
import SummaryCard from "./SummaryCard";
import SpendingByCategoryChart from "../../components/charts/SpendingByCategoryChart";
import IncomeVsExpenseChart from "../../components/charts/IncomeVsExpenseChart";
import MonthlyTrendChart from "../../components/charts/MonthlyTrendChart";

const Dashboard: React.FC = () => {
  return (
    <>
      <title>Dashboard</title>
      <div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
          {/* Summary Cards */}
          <SummaryCard type="Income" />
          <SummaryCard type="Expenses" />
          <SummaryCard type="Balance" />
        </div>

        {/* Charts Placeholder */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold mb-4">Spending by Category</h3>
            <SpendingByCategoryChart />
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold mb-4">Income vs Expenses</h3>
            <IncomeVsExpenseChart />
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold mb-4">Monhtly Trend Line</h3>
            <MonthlyTrendChart />
          </div>
        </div>
      </div>
    </>
  );
};

export default Dashboard;
