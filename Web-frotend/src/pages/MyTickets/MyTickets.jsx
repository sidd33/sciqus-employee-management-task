import { useEffect, useState, useCallback } from "react";
import { Link } from "react-router-dom";
import api from "../../api/axios";
import { getFriendlyErrorMessage } from "../../utils/apiErrors";
import { getCurrentUser, isAdminOrAbove } from "../../auth/roles";
import "./MyTickets.scss";

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

const EMPTY_FORM = { title: "", description: "", departmentId: "" };

function MyTickets() {
  const currentUser = getCurrentUser();

  const [tickets, setTickets] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [form, setForm] = useState(EMPTY_FORM);
  const [formError, setFormError] = useState("");
  const [saving, setSaving] = useState(false);

  const loadTickets = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const res = await api.get(`/customers/${currentUser.id}`);
      setTickets(res.data.tickets || []);
    } catch (err) {
      setError(getFriendlyErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [currentUser.id]);

  const loadDepartments = useCallback(async () => {
    try {
      const res = await api.get("/departments", { params: { pageNumber: 1, pageSize: 100 } });
      setDepartments(res.data.items || []);
    } catch {
      // secondary — create form just shows fewer options
    }
  }, []);

  useEffect(() => {
    loadTickets();
    loadDepartments();
  }, [loadTickets, loadDepartments]);

  function openCreateModal() {
    setForm(EMPTY_FORM);
    setFormError("");
    setShowCreateModal(true);
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setFormError("");
    setSaving(true);
    try {
      await api.post("/tickets", { ...form, customerId: currentUser.id });
      setShowCreateModal(false);
      loadTickets();
    } catch (err) {
      setFormError(getFriendlyErrorMessage(err));
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="my-tickets-page">
      <div className="my-tickets-header">
        <div>
          <h1>My Tickets</h1>
          <p className="my-tickets-subtitle">Track support requests you've submitted</p>
        </div>
        <button className="btn btn--primary" onClick={openCreateModal}>
          + New Ticket
        </button>
      </div>

      {error && <div className="my-tickets-error">{error}</div>}

      {loading ? (
        <div className="my-tickets-loading">Loading your tickets...</div>
      ) : tickets.length === 0 ? (
        <div className="card empty-state">
          <h3>No tickets yet</h3>
          <p>Submit a ticket and our team will get right on it.</p>
        </div>
      ) : (
        <div className="my-tickets-list">
          {tickets.map((ticket) => (
            <Link to={`/tickets/${ticket.id}`} key={ticket.id} className="card card--interactive my-ticket-card">
              <div className="my-ticket-card-header">
                <h3>{ticket.title}</h3>
                <span className={`badge ${STATUS_BADGE[ticket.status] || "badge--inactive"}`}>
                  {STATUS_LABEL[ticket.status] || ticket.status}
                </span>
              </div>
              <p className="my-ticket-description">{ticket.description}</p>
              <div className="my-ticket-card-footer">
                <span>{new Date(ticket.createdAt).toLocaleDateString()}</span>
                {ticket.isSlaBreached && <span className="badge badge--critical">SLA Breached</span>}
              </div>
            </Link>
          ))}
        </div>
      )}

      {showCreateModal && (
        <div className="modal-backdrop" onClick={() => setShowCreateModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>New Ticket</h2>
              <button className="modal-close" onClick={() => setShowCreateModal(false)}>×</button>
            </div>
            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                {formError && <div className="field-error">{formError}</div>}
                <div className="field">
                  <label>Title</label>
                  <input
                    required
                    value={form.title}
                    onChange={(e) => setForm({ ...form, title: e.target.value })}
                  />
                </div>
                <div className="field">
                  <label>Description</label>
                  <textarea
                    required
                    rows={4}
                    value={form.description}
                    onChange={(e) => setForm({ ...form, description: e.target.value })}
                  />
                </div>
                <div className="field">
                  <label>Department</label>
                  <select
                    required
                    value={form.departmentId}
                    onChange={(e) => setForm({ ...form, departmentId: e.target.value })}
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
                  {saving ? "Submitting..." : "Submit Ticket"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default MyTickets;