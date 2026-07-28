import { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2, Building2, Search } from 'lucide-react';
import api from '../../api/axios';
import { getFriendlyErrorMessage } from '../../utils/apiErrors';
import './Departments.scss';

const ICON_THEMES = ['theme-blue', 'theme-green', 'theme-amber', 'theme-purple', 'theme-teal'];

export default function Departments() {
  const [departments, setDepartments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');

  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [formError, setFormError] = useState('');
  const [fieldErrors, setFieldErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [listError, setListError] = useState('');
  const [editingDepartment,setEditingDepartment]=useState(null);

  useEffect(() => { fetchDepartments(); }, []);

 async function fetchDepartments() {
  setLoading(true);
  try {
    const res = await api.get('/departments', { params: { pageSize: 100 } });
    setDepartments(res.data.items);
  } finally {
    setLoading(false);
  }
}

  function closeForm() {
  setShowForm(false);
  setEditingDepartment(null);
  setName('');
  setDescription('');
  setFormError('');
  setFieldErrors({});
}

  function openEdit(department) {
  setEditingDepartment(department);
  setName(department.name);
  setDescription(department.description || '');
  setFormError('');
  setFieldErrors({});
  setShowForm(true);
}

  async function handleSave(e) {
  e.preventDefault();
  setFormError('');
  setFieldErrors({});

  if (!name.trim()) {
    setFieldErrors({ name: 'Department name is required.' });
    return;
  }

  setSaving(true);

  try {
    if (editingDepartment) {
      await api.put(`/departments/${editingDepartment.id}`, {
        name,
        description,
      });
    } else {
      await api.post('/departments', {
        name,
        description,
      });
    }

    closeForm();
    fetchDepartments();
  } catch (err) {
    const backendErrors = err.response?.data?.errors;

    if (backendErrors) {
      const mapped = {};

      Object.entries(backendErrors).forEach(([f, m]) => {
        mapped[f.charAt(0).toLowerCase() + f.slice(1)] =
          Array.isArray(m) ? m[0] : m;
      });

      setFieldErrors(mapped);
    } else {
      setFormError(getFriendlyErrorMessage(err));
    }
  } finally {
    setSaving(false);
  }
}

  async function handleDelete(id, name) {
    if (!window.confirm(`Delete "${name}"? This can't be undone.`)) return;
    setListError('');
    try {
      await api.delete(`/departments/${id}`);
      fetchDepartments();
    } catch (err) {
      setListError(getFriendlyErrorMessage(err));
    }
  }

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return departments;
    return departments.filter(
      (d) => d.name.toLowerCase().includes(q) || d.description?.toLowerCase().includes(q)
    );
  }, [departments, search]);

  return (
    <div className="departments-page">
      <div className="page-header">
        <div>
          <h1>Departments</h1>
          <p>Manage organizational departments.</p>
        </div>
        <button className="btn-primary" onClick={() => setShowForm(true)}>
          <Plus size={18} /> Add Department
        </button>
      </div>

      {departments.length > 0 && (
        <div className="dept-search-box">
          <Search size={16} />
          <input
            type="text"
            placeholder="Search departments..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      )}

      {listError && <p className="error-text">{listError}</p>}

      {loading ? (
        <p className="loading-text">Loading departments...</p>
      ) : filtered.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon"><Building2 size={32} /></div>
          <h3>{departments.length === 0 ? 'No departments yet' : 'No matches found'}</h3>
          <p>{departments.length === 0 ? 'Add your first department to start organizing employees.' : 'Try a different search term.'}</p>
        </div>
      ) : (
        <div className="dept-grid">
          {filtered.map((d, i) => (
            <div className="dept-card" key={d.id}>
              <div className={`dept-icon ${ICON_THEMES[i % ICON_THEMES.length]}`}>
                <Building2 size={20} />
              </div>
              <div className="dept-info">
                <strong>{d.name}</strong>
                <p>{d.description || 'No description'}</p>
              </div>
              <div className="dept-actions">
  <button
    className="edit-btn"
    onClick={() => openEdit(d)}
    title="Edit Department"
  >
    Edit
  </button>

  <button
    className="delete-btn"
    onClick={() => handleDelete(d.id, d.name)}
    title="Delete Department"
  >
    <Trash2 size={16} />
  </button>
</div>
            </div>
          ))}
        </div>
      )}

      {showForm && (
        <div className="form-overlay">
              <form className="dept-form" onSubmit={handleSave} noValidate>
              <div className="form-header">
<h2>{editingDepartment ? 'Edit Department' : 'Add Department'}</h2>

<p>
  {editingDepartment
    ? 'Update department details.'
    : 'Create a new team or department.'}
</p>              <button type="button" className="close-button" onClick={closeForm}>×</button>
            </div>

            {formError && <p className="error-text">{formError}</p>}

            <label>
              Department Name
              <input
                placeholder="e.g. Customer Success"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className={fieldErrors.name ? 'input-error' : ''}
              />
              {fieldErrors.name && <span className="field-error-text">{fieldErrors.name}</span>}
            </label>

            <label>
              Description
              <input
                placeholder="Optional short description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </label>

            <div className="form-actions">
              <button type="button" className="btn-secondary" onClick={closeForm} disabled={saving}>Cancel</button>
              <button type="submit" className="btn-primary" disabled={saving}>{saving
  ? 'Saving...'
  : editingDepartment
    ? 'Update Department'
    : 'Save'}</button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
}