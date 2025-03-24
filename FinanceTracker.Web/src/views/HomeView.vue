<script setup lang="ts">
import { useAccounts } from '../composables/useAccounts';
import Card from 'primevue/card';
import Panel from 'primevue/panel';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Chart from 'primevue/chart';
import { computed, ref } from 'vue';

const { accountSummary, recentTransactions, spendingByCategory, loading, error } = useAccounts();

const chartData = computed(() => {
  return {
    labels: spendingByCategory.value?.map(category => category.category) || [],
    datasets: [
      {
        data: spendingByCategory.value?.map(category => category.amount) || [],
        backgroundColor: [
          '#42A5F5',
          '#66BB6A',
          '#FFA726',
          '#26C6DA',
          '#7E57C2',
          '#EC407A',
          '#EF5350',
          '#78909C',
          '#29B6F6',
          '#AB47BC',
        ],
      },
    ],
  };
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
});
</script>

<template>
  <div class="dashboard">
    <h1>Financial Dashboard</h1>

    <!-- Overview Cards -->
    <div class="grid">
      <div class="col-12 md:col-6 lg:col-3">
        <Card class="overview-card">
          <template #title>Total Balance</template>
          <template #content>
            <div class="card-value positive" v-if="accountSummary">
              ${{ accountSummary.totalBalance.toLocaleString() }}
            </div>
          </template>
        </Card>
      </div>
      <div class="col-12 md:col-6 lg:col-3">
        <Card class="overview-card">
          <template #title>Monthly Income</template>
          <template #content>
            <div class="card-value positive">$3,200.00</div>
          </template>
        </Card>
      </div>
      <div class="col-12 md:col-6 lg:col-3">
        <Card class="overview-card">
          <template #title>Monthly Expenses</template>
          <template #content>
            <div class="card-value negative">$1,850.75</div>
          </template>
        </Card>
      </div>
      <div class="col-12 md:col-6 lg:col-3">
        <Card class="overview-card">
          <template #title>Savings Rate</template>
          <template #content>
            <div class="card-value">42%</div>
          </template>
        </Card>
      </div>
    </div>

    <!-- Account Summary & Spending Section -->
    <div class="grid mt-3">
      <!-- Account Summary -->
      <div class="col-12 lg:col-6">
        <Panel header="Account Summary">
          <DataTable :value="accountSummary?.accounts" stripedRows>
            <Column field="name" header="Account"></Column>
            <Column field="type" header="Type"></Column>
            <Column field="balance" header="Balance">
              <template #body="slotProps">
                ${{ slotProps.data.balance.toLocaleString() }}
              </template>
            </Column>
            <Column>
              <template #body>
                <Button icon="pi pi-eye" class="p-button-text p-button-sm" />
              </template>
            </Column>
          </DataTable>
        </Panel>
      </div>

      <!-- Spending by Category -->
      <div class="col-12 lg:col-6">
        <Panel header="Spending by Category">
          <div class="chart-container">
            <Chart type="doughnut" :data="chartData" :options="chartOptions" />
          </div>
        </Panel>
      </div>
    </div>

    <!-- Recent Transactions -->
    <div class="grid mt-3">
      <div class="col-12">
        <Panel header="Recent Transactions">
          <DataTable :value="recentTransactions" stripedRows>
            <Column field="date" header="Date"></Column>
            <Column field="description" header="Description"></Column>
            <Column field="category" header="Category"></Column>
            <Column field="amount" header="Amount">
              <template #body="slotProps">
                <span :class="slotProps.data.amount >= 0 ? 'positive' : 'negative'">
                  ${{ Math.abs(slotProps.data.amount).toLocaleString() }}
                </span>
              </template>
            </Column>
          </DataTable>
          <div class="flex justify-content-end mt-3">
            <Button label="View All Transactions" icon="pi pi-arrow-right" iconPos="right" />
          </div>
        </Panel>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  width: 100%;
  max-width: 100%;
  padding: 0; /* Remove padding */
  box-sizing: border-box;
}
.dashboard h1 {
  margin-bottom: 1.5rem;
  color: var(--text-color);
}

.overview-card {
  height: 100%;
}

.card-value {
  font-size: 1.5rem;
  font-weight: bold;
}

.positive {
  color: #4caf50;
}

.negative {
  color: #f44336;
}

.chart-container {
  height: 300px;
}
</style>
