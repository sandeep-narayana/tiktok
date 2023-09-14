<template>
  <h1>Home</h1>

  <h3>Count: {{ count }}</h3>

  <button @click="increment">Increment</button>
</template>

<script setup>
import { useUserStore } from "~/store/user";

const store = useUserStore(); // Access the store instance
const count = store;

// Create a method to increment the count
const increment = () => {
  console.log("Index");
  store.increment();
};

// Return variables directly
store, increment;
</script>
