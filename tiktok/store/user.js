import { defineStore } from "pinia";

export const useUserStore = defineStore("user", () => {
  const count = ref(17345);
  const msg = ref("");

  const increment = () => {
    console.log(count);

    count.value = count.value + 1;
    console.log(count);
  };

  async function getMsg() {
    console.log("store");
    try {
      const response = await fetch("https://fakestoreapi.com/products");
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      const data = await response.json();
      console.log(data);
      return data;
    } catch (error) {
      console.error("Error fetching message:", error);
      // Handle the error here, e.g., set an error message
      msg.value = "Failed to fetch message from the API";
    }
  }

  return { count, increment, getMsg };
});
