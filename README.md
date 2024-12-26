## 使用教學

- **users** 節點可對使用者增刪改查
  - `users/` 返回所有使用者名稱
  - `users/add/{你的名稱}` 創建名稱為 你的名稱 的 user
  - `user/del/{name}` 刪除 user

- **map** 節點提供短網址服務
  - `map/list/{name}` 返回 name 下的網址對
  - `map/set/{name}?surl=sss&lurl=https://aot.dapperlib.dev/gettingstarted.html` 設定 name 下的網址映射 sss 到長網址
  - `map/re/{name}/{surl}` 重導向網址到 surl 對應的網址
  - `map/del/{name}/?surl={surl}` 刪除 {surl}
