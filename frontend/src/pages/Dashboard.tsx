import React from 'react';

const Dashboard: React.FC = () => {
  return (
    <div>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        {/* Summary Cards */}
        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-gray-500 text-sm font-medium">Total Income</p>
          <p className="text-3xl font-bold text-green-600 mt-2">$0.00</p>
        </div>
        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-gray-500 text-sm font-medium">Total Expenses</p>
          <p className="text-3xl font-bold text-red-600 mt-2">$0.00</p>
        </div>
        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-gray-500 text-sm font-medium">Balance</p>
          <p className="text-3xl font-bold text-blue-600 mt-2">$0.00</p>
        </div>
      </div>

      {/* Charts Placeholder */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold mb-4">Spending by Category</h3>
          <div className="h-64 flex items-center justify-center text-gray-400">
            Chart coming soon...
          </div>
        </div>
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold mb-4">Income vs Expenses</h3>
          <div className="h-64 flex items-center justify-center text-gray-400">
            Chart coming soon...
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;