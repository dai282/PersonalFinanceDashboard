import React from "react";
import BudgetList from "./BudgetList";

export default function Budgets() {
  return (
    <>
      <title>Budgets</title>
      <div>
        <h3 className="text-xl font-semibold mb-4">Budgets</h3>
        <div className="bg-white rounded-lg shadow p-6">
          <BudgetList />
        </div>
      </div>
    </>
  );
}
