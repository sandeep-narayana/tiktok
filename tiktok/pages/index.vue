<template>
  <h1>Home</h1>

  <h3>Count: {{ count }}</h3>

  <button @click="increment">Increment</button>
  <button @click="get">Get Msg</button>
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

const get = () => {
  console.log(store.getMsg());
};

// Return variables directly
store, increment, get;
</script>
