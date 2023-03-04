# NetCoreWebApiDemo

## 簡介

- 專案主要學習如何使用 NET Core 6 框架的開發，內容包含 API框架、角色登入功能

## 使用技術

- Net Core 6 Web API
- Entity Framework Core
- NLog
- Swagger

## 整體專案結構

- 概念圖
    
    ![image.png](https://github.com/HankLin311/NetCoreWebApiDemo/blob/main/image.png?raw=true)
    
- WebApi
    - Infrastructure (API基礎實作)
    - Params (請求API資料物件)
    - ViewModels (API回傳資料物件)
    - Controller (控制流程)
- Service
    - Implements (流程細節實作)
    - Dtos (WebApi層和Service層傳值的物件)
- Repository
    - Datas (Context物件)
    - DemoDb (包含Demo資料庫的Entities物件、DB實作)
    - Infrastructures (UnitOfWork實作)
- Common
    - 共用方法、定義參數

## 框架說明

- 例外錯誤處理
    - ExceptionFilter :
        - 捕捉自訂業務邏輯相關錯誤訊息
    - Middleware :
        - 捕捉包含 Middleware 或 Controller 中非自訂的錯誤訊息
- Modal Binding 處理
    - 使用框架的 InvalidModelStateResponseFactory ，修改錯誤訊息的回傳內容
    - 驗證條件使用 Net Core 框架的 Attribute 驗證
- Log 處理
    - 資料表記錄 : 使用 ActionFilter 來記錄使用者操作的 API 和輸入輸出
    - 文字檔紀錄 : 處理異常錯誤紀錄，包含 Middleware 和 Controller 內的例外錯誤
- 顯示 Swagger 文件檔
    - Swagger 設定到 Middleware 中 ，可以產生出簡易的API規格文件
- Unit Of Work 作為資料存取的封裝
    - 將資料庫做封裝，減少在 Service 層中寫入許多的存取資料的程式碼
    - 減少 Service 層注入 N 個 Repository
- 自訂角色權限驗證
    - 第一層授權驗證 (AuthorizationFilter)
        - 驗證 JWT 判斷登入是否合法，如果合法的話就可以使用該服務
    - 第二層權限驗證
        - 驗證 JWT 判斷角色是否合法 (排除驗證過期)， 如果合法可以使用該服務
        - 另外新增一個 Attribute，讓 API 可以接受匿名使用

## 如何使用此 API

- Login ⇒ Logout
- Login ⇒ RefreshLoginToken
- Register ⇒ Login(Role: Admin) ⇒ GetRegisterUsers / GetRoleName ⇒ ApporoveRegister

## 參考

- [https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31](https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31)
- [https://raychiutw.github.io/2019/隨手-Design-Pattern-4-Repository-模式-Repository-Pattern/](https://raychiutw.github.io/2019/%E9%9A%A8%E6%89%8B-Design-Pattern-4-Repository-%E6%A8%A1%E5%BC%8F-Repository-Pattern/)
- [https://raychiutw.github.io/2019/隨手-Design-Pattern-2-軟體分層設計模式-Software-Layered-Architecture-Pattern/](https://raychiutw.github.io/2019/%E9%9A%A8%E6%89%8B-Design-Pattern-2-%E8%BB%9F%E9%AB%94%E5%88%86%E5%B1%A4%E8%A8%AD%E8%A8%88%E6%A8%A1%E5%BC%8F-Software-Layered-Architecture-Pattern/)
- [https://ithelp.ithome.com.tw/articles/10242295](https://ithelp.ithome.com.tw/articles/10242295)
- [https://ithelp.ithome.com.tw/articles/10195407](https://ithelp.ithome.com.tw/articles/10195407)
- [https://www.youtube.com/watch?v=4kmkPKwLLNE&list=PL9sJKk6XPMxehYCui7OysUV6trlBbJ4T_](https://www.youtube.com/watch?v=4kmkPKwLLNE&list=PL9sJKk6XPMxehYCui7OysUV6trlBbJ4T_)
