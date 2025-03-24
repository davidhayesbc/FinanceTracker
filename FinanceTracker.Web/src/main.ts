import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config'
import App from './App.vue'
import router from './router'

// Import PrimeVue components
import Button from "primevue/button"
import Menubar from 'primevue/menubar'
import Menu from 'primevue/menu'
import Card from 'primevue/card'
import Panel from 'primevue/panel'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Chart from 'primevue/chart'

// Import PrimeVue styles
import Aura from '@primeuix/themes/aura'
import 'primeicons/primeicons.css'   // icons
import 'primeflex/primeflex.css'     // grid system

const app = createApp(App)

// Register PrimeVue components
app.component('Button', Button)
app.component('Menubar', Menubar)
app.component('Menu', Menu)
app.component('Card', Card)
app.component('Panel', Panel)
app.component('DataTable', DataTable)
app.component('Column', Column)
app.component('Chart', Chart)

app.use(createPinia())
app.use(router)
app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: {
            prefix: 'p',
            darkModeSelector: 'system',
            cssLayer: false
        }
    }
})

app.mount('#app')

