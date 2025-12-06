import { useAuth0 } from "@auth0/auth0-react";
import React from "react";
import { Navigate } from "react-router-dom";
//import { useAuth } from "../contexts/AuthContext";

interface PrivateRouteProps {
  children: React.ReactElement;
}

// A component that protects routes that require authentication
const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
  //const { user, isLoading } = useAuth();

  //now using auth0
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    loginWithRedirect();
    return null;
  }

  return children;

  //return user ? children : <Navigate to="/login" replace />;
};

export default PrivateRoute;
