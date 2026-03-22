import { useEffect, useState } from 'react';
import { 
  Container, Typography, Box, Button, Grid, Card, CardContent, 
  CardActions, Chip, IconButton, CircularProgress, Alert, Fab, Paper,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField
} from '@mui/material';
import { 
  Add as AddIcon, 
  Delete as DeleteIcon, 
  CheckCircle as CheckIcon,
  RadioButtonUnchecked as TodoIcon,
  Edit as EditIcon
} from '@mui/icons-material';
import api from '@/api/axiosConfig';
import { useAuth } from '@/context/AuthContext';
import { type TaskItem } from '@/types';

export const Dashboard = () => {
  const { user, logout } = useAuth();
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Modal and Form State
  const [open, setOpen] = useState(false);
  const [modalError, setModalError] = useState('');
  const [editingTask, setEditingTask] = useState<TaskItem | null>(null);
  const [newTask, setNewTask] = useState({ title: '', description: '', dueDate: '' });
  
  const today = new Date().toLocaleDateString('en-CA');

  const fetchTasks = async () => {
    try {
      setLoading(true);
      const response = await api.get('/tasks'); 
      setTasks(response.data);
    } catch (err: any) {
      console.error('Failed to fetchTasks:', err);
      setError('Failed to load tasks. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleEditClick = (task: TaskItem) => {
    setEditingTask(task);
    setNewTask({
      title: task.title,
      description: task.description || '',
      // Ensure we only take the YYYY-MM-DD part for the date input
      dueDate: task.dueDate.split('T')[0],
    });
    setOpen(true);
  };

  const handleSaveTask = async () => {
    setModalError('');

    if (!newTask.title || !newTask.dueDate) {
      setModalError("Title and Due Date are required.");
      return;
    }

    try {
      if (editingTask) {
        // UPDATE MODE
        const updateDto = { 
          ...newTask, 
          status: editingTask.status 
        };
        await api.put(`/tasks/${editingTask.id}`, updateDto);
        
        setTasks(tasks.map(t => t.id === editingTask.id 
          ? { ...t, ...newTask } 
          : t
        ));
      } else {
        // CREATE MODE
        const response = await api.post('/tasks', newTask);
        setTasks([...tasks, response.data]);
      }
      handleCloseDialog();
    } catch (err: any) {
      const message = err.response?.data?.error || 'Failed to save task';
      setModalError(message); 
    }
  };

  const handleCloseDialog = () => {
    setOpen(false);
    setEditingTask(null);
    setModalError(''); 
    setNewTask({ title: '', description: '', dueDate: '' });
  };

  const handleDelete = async (id: number) => {
    try {
      await api.delete(`/tasks/${id}`);
      setTasks(tasks.filter(t => t.id !== id));
    } catch (err) {
      console.error('Failed to delete task:', err);
      alert('Error deleting task. Please check your connection.');
    }
  };

  const toggleStatus = async (task: TaskItem) => {
    const newStatus = task.status === 'Completed' ? 'Pending' : 'Completed';
    try {
      const updateDto = {
        title: task.title,
        description: task.description,
        dueDate: task.dueDate,
        status: newStatus
      };
      
      await api.put(`/tasks/${task.id}`, updateDto);
      setTasks(tasks.map(t => t.id === task.id ? { ...t, status: newStatus } : t));
    } catch (err) {
      console.error('Failed to update task status:', err);
      alert('Error updating task. The server might be unavailable.');
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          My Tasks
        </Typography>
        <Box>
          <Typography variant="subtitle1" sx={{ display: 'inline', mr: 2 }}>
            Hi, {user?.username}
          </Typography>
          <Button variant="outlined" color="inherit" onClick={logout}>
            Logout
          </Button>
        </Box>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>{error}</Alert>}

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Grid container spacing={3}>
          {tasks.length === 0 ? (
            <Grid size={12}>
              <Paper sx={{ p: 5, textAlign: 'center', bgcolor: '#f5f5f5' }}>
                <Typography color="textSecondary">
                  No tasks found. Click the + button to create one!
                </Typography>
              </Paper>
            </Grid>
          ) : (
            tasks.map((task) => (
              <Grid size={{ xs: 12, sm: 6, md: 4 }} key={task.id}>                
                <Card elevation={2} sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                      <Chip 
                        label={task.status} 
                        size="small" 
                        color={task.status === 'Completed' ? 'success' : 'primary'} 
                      />
                      <Typography variant="caption" color="textSecondary">
                        Due: {new Date(task.dueDate).toLocaleDateString()}
                      </Typography>
                    </Box>
                    <Typography variant="h6" component="h2" gutterBottom sx={{ mt: 1 }}>
                      {task.title}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                      {task.description || 'No description provided.'}
                    </Typography>
                  </CardContent>
                  <CardActions sx={{ justifyContent: 'space-between', px: 2, pb: 2 }}>
                    <Box>
                      <IconButton onClick={() => toggleStatus(task)} color="primary" title="Toggle Status">
                        {task.status === 'Completed' ? <CheckIcon /> : <TodoIcon />}
                      </IconButton>
                      <IconButton onClick={() => handleEditClick(task)} color="info" title="Edit Task">
                        <EditIcon />
                      </IconButton>
                    </Box>
                    <IconButton onClick={() => handleDelete(task.id)} color="error" title="Delete Task">
                      <DeleteIcon />
                    </IconButton>
                  </CardActions>
                </Card>
              </Grid>
            ))
          )}
        </Grid>
      )}

      <Fab 
        color="primary" 
        sx={{ position: 'fixed', bottom: 32, right: 32 }}
        onClick={() => setOpen(true)}
      >
        <AddIcon />
      </Fab>

      <Dialog open={open} onClose={handleCloseDialog} fullWidth maxWidth="xs">
        <DialogTitle>{editingTask ? 'Edit Task' : 'Create New Task'}</DialogTitle>
        <DialogContent>
          {modalError && (
            <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{modalError}</Alert>
          )}
          
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Title"
              fullWidth
              required
              value={newTask.title}
              onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
            />
            <TextField
              label="Description"
              fullWidth
              multiline
              rows={3}
              value={newTask.description}
              onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
            />
            <TextField
              label="Due Date"
              type="date"
              fullWidth
              required
              slotProps={{
                inputLabel: { shrink: true },
                htmlInput: { min: today } 
              }}
              value={newTask.dueDate}
              onChange={(e) => setNewTask({ ...newTask, dueDate: e.target.value })}
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSaveTask} variant="contained">
            {editingTask ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};