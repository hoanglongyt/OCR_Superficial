// db.js
import { openDB } from 'idb';

const DB_NAME = 'MyImageDB';
const STORE_NAME = 'images';

export async function initDB() {
  return openDB(DB_NAME, 1, {
    upgrade(db) {
      if (!db.objectStoreNames.contains(STORE_NAME)) {
        db.createObjectStore(STORE_NAME);
      }
    },
  });
}

export async function saveImage(key, file) {
  const db = await initDB();
  await db.put(STORE_NAME, file, key); // key can be a string like "profile"
}

export async function getImage(key) {
  const db = await initDB();
  return await db.get(STORE_NAME, key);
}

export async function deleteImage(key) {
  const db = await initDB();
  await db.delete(STORE_NAME, key);
}
