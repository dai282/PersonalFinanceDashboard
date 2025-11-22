import React from 'react';
import CategoryList from './CategoryList';

const Categories: React.FC = () => {
  return (
    <div>
      <h3 className="text-xl font-semibold mb-4">Categories</h3>
      <div className="bg-white rounded-lg shadow p-6">
        <CategoryList />
      </div>
    </div>
  );
};

export default Categories;