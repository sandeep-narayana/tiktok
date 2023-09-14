import { defineStore } from "pinia";

export const useUserStore = defineStore("user", () => {
  const count = ref(17345);
  function increment() {
    console.log(count)

    count.value = count.value+1;
    console.log(count)
  }

  return { count, increment };
});
