' Filename: GlobalVarsModule.vb
Module GlobalVarsModule
    ' Database Connection String (Gamitin lang ang isang deklarasyon dito)
    ' PALITAN ANG MGA DETAILS SA IBABA NG IYONG ACTUAL DATABASE CONFIGURATION.
    Public connectionString As String = "Server=localhost;Database=laybsis_dbs;Uid=root;Pwd="

    ' Ito ang magse-save ng role ng user (hal. "Borrower", "Staff", "Guest")
    Public CurrentUserRole As String = "Guest"

    ' Idinagdag para i-store ang LRN/EmployeeNo ng user pagkatapos mag-login
    Public CurrentUserID As String = ""
End Module