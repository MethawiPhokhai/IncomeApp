import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login/Login.vue'
import Dashboard from '../views/Dashboard/Dashboard.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'login',
            component: Login
        },
        {
            path: '/dashboard',
            name: 'dashboard',
            component: Dashboard,
            meta: { requiresAuth: true }
        }
    ]
})

// Navigation guard for protected routes
router.beforeEach((to, _from, next) => {
    const token = localStorage.getItem('token');

    if (to.meta.requiresAuth && !token) {
        // Redirect to login if not authenticated
        next({ name: 'login' });
    } else if (to.name === 'login' && token) {
        // Redirect to dashboard if already authenticated
        next({ name: 'dashboard' });
    } else {
        next();
    }
});

export default router
