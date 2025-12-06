//auth0 handles login

// import React, { useState } from "react";
// import { useNavigate, Link } from "react-router-dom";
// import { useAuth } from "../contexts/AuthContext";

// const Login: React.FC = () => {
//   const [email, setEmail] = useState("");
//   const [password, setPassword] = useState("");
//   const [error, setError] = useState("");
//   const [isLoading, setIsLoading] = useState(false);
//   const { login } = useAuth();
//   const navigate = useNavigate();

//   const handleSubmit = async (e: React.FormEvent) => {
//     e.preventDefault();
//     e.stopPropagation();
//     setError("");
//     setIsLoading(true);

//     try {
//       await login(email, password);
//       navigate("/dashboard");
//     } catch (err: any) {
//       console.error("Login error:", err); // Add this for debugging
//       const errorMessage =
//         err.response?.data?.message ||
//         err.response?.data ||
//         "Login failed. Please check your credentials.";
//       console.log("Setting error message:", errorMessage);
//       setError(errorMessage);
//     } finally {
//       // Use a 'finally' block to ensure the loading state is always reset.
//       setIsLoading(false);
//     }
//   };

//   return (
//     <>
      
//       <div className="min-h-screen flex items-center justify-center bg-gray-100">
//         <div className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
//           <h2 className="text-3xl font-bold text-center mb-6 text-gray-800">
//             Login
//           </h2>
//           {error && (
//             <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
//               {error}
//             </div>
//           )}
//           <form onSubmit={handleSubmit}>
//             <div className="mb-4">
//               <label className="block text-gray-700 text-sm font-medium mb-2">
//                 Email
//               </label>
//               <input
//                 type="email"
//                 value={email}
//                 onChange={(e) => setEmail(e.target.value)}
//                 required
//                 className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
//               />
//             </div>
//             <div className="mb-6">
//               <label className="block text-gray-700 text-sm font-medium mb-2">
//                 Password
//               </label>
//               <input
//                 type="password"
//                 value={password}
//                 onChange={(e) => setPassword(e.target.value)}
//                 required
//                 className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
//               />
//             </div>
//             <button
//               type="submit"
//               disabled={isLoading}
//               className="w-full bg-blue-500 text-white py-2 rounded-lg hover:bg-blue-600 transition-colors disabled:bg-blue-300"
//             >
//               {isLoading ? "Logging in..." : "Login"}
//             </button>
//           </form>
//           <p className="text-center mt-4 text-gray-600">
//             Don't have an account?{" "}
//             <Link to="/register" className="text-blue-500 hover:underline">
//               Register
//             </Link>
//           </p>
//         </div>
//       </div>
//     </>
//   );
// };

// export default Login;

export {};