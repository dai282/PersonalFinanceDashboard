import React, { useState } from 'react';
import TransactionList from '../components/TransactionList';
import TransactionForm from '../components/TransactionForm';

const Transactions: React.FC = () => {
  const [isFormVisible, setIsFormVisible] = useState(false);

  const handleCreateTransaction = () => {
    setIsFormVisible(true);
  };

  const handleCloseForm = () => {
    setIsFormVisible(false);
  };

  return (
    <>
      <title>Transactions</title>
      <div className="bg-white rounded-lg shadow p-6">
        <div className='flex items-center justify-between mt-4'>
          <p className="text-2xl font-bold mb-4">Transactions</p>
          <button
            onClick={handleCreateTransaction}
            className='bg-green-400 text-white font-bold py-2 px-4 rounded'
          >
            Create Transaction
          </button>
        </div>
        <TransactionList />
      </div>

      {isFormVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-2xl">
            <TransactionForm onClose={handleCloseForm} />
          </div>
        </div>
      )}
    </>
  );
};

export default Transactions;
