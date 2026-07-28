import { useEffect, useState, useCallback } from "react";
import { Link } from "react-router-dom";
import api from "../../api/axios";
import { getCurrentUser, isAdminOrAbove } from "../../auth/roles";
import { getFriendlyErrorMessage } from "../../utils/apiErrors";
import "./Tickets.scss";

const PAGE_SIZE = 10;

const STATUS_OPTIONS = ["Unassigned", "Assigned", "InProgress", "Completed", "Closed", "Reopened"];

const STATUS_BADGE = {
  Unassigned: "badge--critical",
  Assigned: "badge--warning",
  InProgress: "badge--high-priority",
  Completed: "badge--success",
  Closed: "badge--inactive",
  Reopened: "badge--warning",
};

const STATUS_LABEL = {
  Unassigned: "Unassigned",
  Assigned: "Assigned",
  InProgress: "In Progress",
  Completed: "Completed",
  Closed: "Closed",
  Reopened: "Reopened",
};

const EMPTY_CREATE_FORM = { title: "", description: "", customerId: "", departmentId: "" };

function Tickets() {
  const currentUser = getCurrentUser();
  const canManage = isAdminOrAbove(currentUser);

  const [tickets, setTickets] = useState([]);
  const [hasNextPage, setHasNextPage] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);

  const [statusFilter, setStatusFilter] = useState("");
  const [departmentFilter, setDepartmentFilter] = useState("");
  const [searchTerm, setSearchTerm] = useState("");

  const [departments, setDepartments] = useState([]);
  const [employeesByDept, setEmployeesByDept] = useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [createForm, setCreateForm] = useState(EMPTY_CREATE_FORM);
  const [createError, setCreateError] = useState("");
  const [saving, setSaving] = useState(false);

  const [assignTarget, setAssignTarget] = useState(null);
  const [assignEmployeeId, setAssignEmployeeId] = useState("");
  const [assignError, setAssignError] = useState("");
  const [assigning, setAssigning] = useState(false);

  const loadDepartments = useCallback(async () => {
    try {
      const res = await api.get("/departments", { params: { pageNumber: 1, pageSize: 100 } });
      setDepartments(res.data.items || []);
    } catch {
      // secondary data — filters/create form just show fewer options
    }
  }, []);

  const loadTickets = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const res = await api.get("/tickets", {
        params: {
          pageNumber,
          pageSize: PAGE_SIZE + 1, // fetch one extra to detect a next page
          status: statusFilter || undefined,
          departmentId: departmentFilter || undefined,
          searchTerm: searchTerm || undefined,
        },
      });
      const items = res.data || [];
      const next = items.length > PAGE_SIZE;
      setTickets(next ? items.slice(0, PAGE_SIZE) : items);
      setHasNextPage(next);
    } catch (err) {
      setError(getFriendlyErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [pageNumber, statusFilter, departmentFilter, searchTerm]);

  useEffect(() => {
    loadDepartments();
  }, [loadDepartments]);

  useEffect(() => {
    loadTickets();
  }, [loadTickets]);

  useEffect(() => {
    setPageNumber(1);
  }, [statusFilter, departmentFilter, searchTerm]);

  function openCreateModal() {
    setCreateForm(EMPTY_CREATE_FORM);
    setCreateError("");
    setShowCreateModal(true);
  }

  async function handleCreateSubmit(e) {
    e.preventDefault();
    setCreateError("");
    setSaving(true);
    try {
      await api.post("/tickets", createForm);
      setShowCreateModal(false);
      loadTickets();
    } catch (err) {
      setCreateError(getFriendlyErrorMessage(err));
    } finally {
      setSaving(false);
    }
  }

  async function openAssignModal(ticket) {
    setAssignTarget(ticket);
    setAssignEmployeeId("");
    setAssignError("");
    try {
      const res = await api.get("/employees", {
        params: { pageNumber: 1, pageSize: 100, departmentId: ticket.departmentId },
      });
      setEmployeesByDept((res.data.items || []).filter((e) => e.isActive));
    } catch {
      setEmployeesByDept([]);
    }
  }

  async function handleAssignSubmit(e) {
    e.preventDefault();
    setAssignError("");
    setAssigning(true);
    try {
      await api.patch(`/tickets/${assignTarget.id}/assign`, { employeeId: assignEmployeeId });
      setAssignTarget(null);
      loadTickets();
    } catch (err) {
      setAssignError(getFriendlyErrorMessage(err));
    } finally {
      setAssigning(false);
    }
  }

  return (
    <div className="tickets-page">
      <div className="tickets-header">
        <div>
          <h1>Tickets</h1>
          <p className="tickets-subtitle">Support requests across all departments</p>
        </div>
        {canManage && (
          <button className="btn btn--primary" onClick={openCreateModal}>
            + New Ticket
          </button>
        )}
      </div>

      <div className="tickets-filters">
        <input
          type="text"
          placeholder="Search tickets..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All statuses</option>
          {STATUS_OPTIONS.map((s) => (
            <option key={s} value={s}>{STATUS_LABEL[s]}</option>
          ))}
        </select>
        <select value={departmentFilter} onChange={(e) => setDepartmentFilter(e.target.value)}>
          <option value="">All departments</option>
          {departments.map((d) => (
            <option key={d.id} value={d.id}>{d.name}</option>
          ))}
        </select>
      </div>

      {error && <div className="tickets-error">{error}</div>}

      <div className="card tickets-table-card">
        {loading ? (
          <div className="tickets-loading">Loading tickets...</div>
        ) : tickets.length === 0 ? (
          <div className="empty-state">
            <h3>No tickets found</h3>
            <p>Try adjusting your search or filters.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Customer</th>
                <th>Status</th>
                <th>Created</th>
                <th>SLA</th>
                <th></th>
              </tr>
            </thead>
           <tbody>
  {tickets.map((ticket) => {
    console.log("Ticket object:", ticket);

    return (
      <tr key={ticket.id}>
        <td>
          <Link to={`/tickets/${ticket.id}`} className="ticket-title-link">
            {ticket.title}
          </Link>
        </td>

        <td>{ticket.customerName}</td>

        <td>
          <span className={`badge ${STATUS_BADGE[ticket.status] || "badge--inactive"}`}>
            {STATUS_LABEL[ticket.status] || ticket.status}
          </span>
        </td>

        <td>{new Date(ticket.createdAt).toLocaleDateString()}</td>

        <td>
          {ticket.isSlaBreached ? (
            <span className="badge badge--critical">Breached</span>
          ) : (
            <span className="badge badge--success">On Track</span>
          )}
        </td>

        <td>
          <div className="row-actions">
            <Link to={`/tickets/${ticket.id}`} className="btn btn--ghost">
              View
            </Link>
          </div>
        </td>
      </tr>
    );
  })}
</tbody>
          </table>
        )}
      </div>

      {(pageNumber > 1 || hasNextPage) && (
        <div className="tickets-pagination">
          <button
            className="btn btn--secondary"
            disabled={pageNumber <= 1}
            onClick={() => setPageNumber((p) => p - 1)}
          >
            Previous
          </button>
          <span>Page {pageNumber}</span>
          <button
            className="btn btn--secondary"
            disabled={!hasNextPage}
            onClick={() => setPageNumber((p) => p + 1)}
          >
            Next
          </button>
        </div>
      )}

      {showCreateModal && (
        <div className="modal-backdrop" onClick={() => setShowCreateModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>New Ticket</h2>
              <button className="modal-close" onClick={() => setShowCreateModal(false)}>×</button>
            </div>
            <form onSubmit={handleCreateSubmit}>
              <div className="modal-body">
                {createError && <div className="field-error">{createError}</div>}
                <div className="field">
                  <label>Title</label>
                  <input
                    required
                    value={createForm.title}
                    onChange={(e) => setCreateForm({ ...createForm, title: e.target.value })}
                  />
                </div>
                <div className="field">
                  <label>Description</label>
                  <textarea
                    required
                    rows={4}
                    value={createForm.description}
                    onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
                  />
                </div>
                <div className="field">
                  <label>Customer ID</label>
                  <input
                    required
                    placeholder="Customer GUID"
                    value={createForm.customerId}
                    onChange={(e) => setCreateForm({ ...createForm, customerId: e.target.value })}
                  />
                </div>
                <div className="field">
                  <label>Department</label>
                  <select
                    required
                    value={createForm.departmentId}
                    onChange={(e) => setCreateForm({ ...createForm, departmentId: e.target.value })}
                  >
                    <option value="">Select department</option>
                    {departments.map((d) => (
                      <option key={d.id} value={d.id}>{d.name}</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn--secondary" onClick={() => setShowCreateModal(false)}>Cancel</button>
                <button type="submit" className="btn btn--primary" disabled={saving}>
                  {saving ? "Creating..." : "Create Ticket"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {assignTarget && (
        <div className="modal-backdrop" onClick={() => setAssignTarget(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Assign Ticket</h2>
              <button className="modal-close" onClick={() => setAssignTarget(null)}>×</button>
            </div>
            <form onSubmit={handleAssignSubmit}>
              <div className="modal-body">
                {assignError && <div className="field-error">{assignError}</div>}
                <p className="assign-ticket-title">{assignTarget.title}</p>
                <div className="field">
                  <label>Employee</label>
                  <select
                    required
                    value={assignEmployeeId}
                    onChange={(e) => setAssignEmployeeId(e.target.value)}
                  >
                    <option value="">Select employee</option>
                    {employeesByDept.map((emp) => (
                      <option key={emp.id} value={emp.id}>
                        {emp.firstName} {emp.lastName}
                      </option>
                    ))}
                  </select>
                  {employeesByDept.length === 0 && (
                    <span className="field-error" style={{ color: "inherit", opacity: 0.7 }}>
                      No active employees available in this ticket's department.
                    </span>
                  )}
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn--secondary" onClick={() => setAssignTarget(null)}>Cancel</button>
                <button type="submit" className="btn btn--primary" disabled={assigning || !assignEmployeeId}>
                  {assigning ? "Assigning..." : "Assign"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Tickets;