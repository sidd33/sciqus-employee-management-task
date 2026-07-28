export function getRoleLabel(user) {
  if (!user) return "";
  if (typeof user.role === "string") return user.role;
  if (user.role === 3) return "SuperAdmin";
  if (user.role === 2) return "Admin";
  if (user.role === 1) return "Employee";
  return "Employee";
}

export function isAdmin(user) {
  if (!user) return false;
  return (
    user.role === "Admin" ||
    user.role === "admin" ||
    user.role === "SuperAdmin" ||
    user.role === "superadmin" ||
    user.role === 2 ||
    user.role === 3
  );
}

export function isCustomer(user) {
  if (!user) return false;
  return user.role === "Customer" || user.role === "customer";
}

export function isAgent(user) {
  if (!user) return false;
  return user.role === "Employee" || user.role === "employee" || user.role === 1;
}

// Admin and SuperAdmin both count as "can manage" for ticket/employee actions
export function isAdminOrAbove(user) {
  return isAdmin(user);
}

// Reads the cached user object set at login time
export function getCurrentUser() {
  return JSON.parse(localStorage.getItem('user') || 'null');
}