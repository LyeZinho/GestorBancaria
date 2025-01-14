﻿Imports System.Data.SQLite



Public Class Cliente
    'Atributos clientes
    Private inome As String
    Private iidade As Integer
    Private inif As Integer
    Private isaldo As Double
    Private ihashPassword As String = ""
    Private ipasswordString As String = ""
    Private userIsValid As Boolean

    'Variaves de controle
    Private hashIsDone As Boolean = False

    '[PROPERTY]
    Property Nome() As String
        Get
            Nome = inome
        End Get
        Set(ByVal Vaule As String)
            Dim result As Integer = FindSQLI(Vaule)
            If result = 0 Then
                userIsValid = False
            ElseIf result = 1 Then
                userIsValid = True
                inome = Vaule
            End If
        End Set
    End Property

    Property PasswordString() As String
        Get
            PasswordString = ipasswordString
        End Get
        Set(ByVal Vaule As String)
            Dim result As Integer = FindSQLI(Vaule)
            If result = 0 Then
                userIsValid = False
            ElseIf result = 1 Then
                userIsValid = True
                ipasswordString = Vaule
            End If
        End Set
    End Property

    Property Idade() As Integer
        Get
            Idade = iidade
        End Get
        Set(ByVal Vaule As Integer)
            If Idade < 18 Then
                userIsValid = False
            ElseIf Vaule = Nothing Then
                userIsValid = False
            ElseIf Idade < 0 Then
                userIsValid = False
            Else
                userIsValid = True
                iidade = Vaule
            End If
        End Set
    End Property


    Property Nif() As Integer
        Get
            Nif = inif
        End Get
        Set(ByVal Vaule As Integer)
            If Nif < 100000000 Then
                userIsValid = False
            ElseIf Nif > 999999999 Then
                userIsValid = False
            ElseIf Nif = Nothing Then
                userIsValid = False
            Else
                userIsValid = True
                inif = Vaule
            End If
        End Set
    End Property


    '[SECURITY]
    Public Function FindSQLI(_input) As Integer
        Dim Filter As String = "insert INSERT where WHERE if IF select SELECT update UPDATE delete DELETE create CREATE null NULL"
        Dim result As Integer = Filter.IndexOf(_input)
        '0 = confirmed sql injection
        '1 = not confimed sql injection
        If result = -1 Then
            Return 0
        ElseIf Not (result = -1) Then
            Return 1
        End If
        Return 3
    End Function

    Private Function VerifyHash(_hashvar) As Boolean
        If _hashvar = "" Then
            Return False
        End If
        Return True
    End Function

    Public Function Sha1(_imput) As String
        Dim sha1Obj As New Security.Cryptography.SHA1CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(_imput)

        bytesToHash = sha1Obj.ComputeHash(bytesToHash)

        Dim strResult As String = ""

        For Each b As Byte In bytesToHash
            strResult += b.ToString("x2")
        Next
        Return strResult
    End Function





    '[SQL-CRUD]
    Private Shared Sub NewClientTable()
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=clients.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS Clientes (nome CHAR(255), idade INT, nif INT, saldo DOUBLE PRECISION, hashpass CHAR(255));"
        sqlite_cmd.ExecuteNonQuery()
    End Sub

    Private Sub InsertIntoClientes()
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=clients.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "INSERT INTO Clientes (nome, idade, nif, saldo, hashpass) VALUES ('" + inome + "'," + iidade.ToString + "," + inif.ToString + "," + isaldo.ToString + ",'" + ihashPassword + "');"
        sqlite_cmd.ExecuteNonQuery()
    End Sub

    Private Function ReadSaldoClientes(_nome_cliente As String) As Double
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        Dim sqlite_datareader As SQLiteDataReader
        sqlite_conn = New SQLiteConnection("Data Source=database.sqlite;Version=3;New=True;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "SELECT saldo FROM Clientes WHERE nome =" + _nome_cliente + ";"
        sqlite_datareader = sqlite_cmd.ExecuteReader()
        Dim saldoReader As Double
        Dim SystemAlerts As New SystemAlerts()
        Try
            While (sqlite_datareader.Read())
                saldoReader = sqlite_datareader.GetDouble(0)
            End While
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
        Return saldoReader
    End Function

    Private Function ReadHashPassClientes(_nome_cliente As String) As Double
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        Dim sqlite_datareader As SQLiteDataReader
        sqlite_conn = New SQLiteConnection("Data Source=database.sqlite;Version=3;New=True;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "SELECT hashpass FROM Clientes WHERE nome =" + _nome_cliente + ";"
        sqlite_datareader = sqlite_cmd.ExecuteReader()
        Dim hashreader As String
        Dim SystemAlerts As New SystemAlerts()
        Try
            While (sqlite_datareader.Read())
                hashreader = sqlite_datareader.GetString(0)
            End While
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
        Return hashreader
    End Function

    Private Function ReadNomeClientes(_nif_cliente As Integer) As String
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        Dim sqlite_datareader As SQLiteDataReader
        sqlite_conn = New SQLiteConnection("Data Source=database.sqlite;Version=3;New=True;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "SELECT nome FROM Clientes WHERE nif =" + _nif_cliente.ToString + ";"
        sqlite_datareader = sqlite_cmd.ExecuteReader()
        Dim nomeReader As Double
        Dim SystemAlerts As New SystemAlerts()
        Try
            While (sqlite_datareader.Read())
                nomeReader = sqlite_datareader.GetInt32(0)
            End While
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
        Return nomeReader
    End Function

    Private Sub UpdateSaldoClientes(_nome_cliente As String, _saldo_novo As Double)
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=clients.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "UPDATE Clientes SET saldo = " + _saldo_novo.ToString + " WHERE nome = " + _nome_cliente + ";"
        sqlite_cmd.ExecuteNonQuery()
    End Sub

    Private Sub UpdatePassHashClientes(_nome_cliente As String)
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=clients.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "UPDATE Clientes SET hashpass = " + ihashPassword + " WHERE nome = " + _nome_cliente + ";"
        sqlite_cmd.ExecuteNonQuery()
    End Sub

    '[PASSWORD]
    Public Sub HashClientePassword()
        Dim SystemAlerts As New SystemAlerts()
        Try
            Dim hash = Sha1(ipasswordString)
            ihashPassword = hash
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
    End Sub

    Public Function ValidatePassword(_password, _nome_cliente) As Boolean
        Dim SystemAlerts As New SystemAlerts()
        Dim hashgen = Sha1(_password)
        Dim dbhashpass = ReadHashPassClientes(_nome_cliente)
        Try
            If hashgen = dbhashpass Then
                Return True
            ElseIf Not hashgen = dbhashpass Then
                Return False
            End If
        Catch ex As Exception
            Return False
            SystemAlerts.ErrorBox(ex)
        End Try
    End Function


    '[UPDATE/INSERT]
    Public Function UpdateClienteVaules() As String
        Dim SystemAlerts As New SystemAlerts()
        hashIsDone = VerifyHash(ihashPassword)

        Try
            If userIsValid = True Then
                InsertIntoClientes()
                Return "Concluido!"
            ElseIf (userIsValid = False) Or (hashIsDone = False) Or (userIsValid = False And hashIsDone = False) Then
                Return "Os dados são invalidos logo o cliente não foi atualizado!"
            End If
        Catch ex As Exception
            Return ex.ToString
            SystemAlerts.ErrorBox(ex)
        End Try
        Return "..."
    End Function

End Class


Public Class LoginClientes
    Inherits Cliente

    '[ATRIBUTOS]
    Private iusername As String
    Private ihashpassword As String
    Private ipassword As String
    Private inif As Integer
    Private userIsValid As Boolean
    Private hashisdone As Boolean

    '[PROPERTY]
    Property Username() As String
        Get
            Username = iusername
        End Get
        Set(ByVal Vaule As String)
            Dim result As Integer = FindSQLI(Vaule)
            If result = 0 Then
                userIsValid = False
            ElseIf result = 1 Then
                userIsValid = True
                ihashpassword = Vaule
            End If
        End Set
    End Property

    Property Password() As String
        Get
            Password = ipassword
        End Get
        Set(ByVal Vaule As String)
            Dim result As Integer = FindSQLI(Vaule)
            If result = 0 Then
                userIsValid = False
            ElseIf result = 1 Then
                userIsValid = True
                ipassword = Vaule
            End If
        End Set
    End Property


    '[PASSWORD]
    Public Sub HashClientLoginPassword()
        Dim SystemAlerts As New SystemAlerts()
        Try
            Dim hash = Sha1(ipassword)
            ipassword = hash
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
    End Sub

    Public Function ValidatePassword(_password, _nome_cliente) As Boolean
        Dim SystemAlerts As New SystemAlerts()
        Dim hashgen = Sha1(_password)
        Dim dbhashpass = ReadHashPassClientLogin(_nome_cliente)
        Try
            If hashgen = dbhashpass Then
                Return True
            ElseIf Not hashgen = dbhashpass Then
                Return False
            End If
        Catch ex As Exception
            Return False
            SystemAlerts.ErrorBox(ex)
        End Try
    End Function

    Private Function VerifyHash(_hashvar) As Boolean
        If _hashvar = "" Then
            Return False
        End If
        Return True
    End Function


    '[SQL]
    Private Shared Sub NewClientLoginTable()
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=login.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS Login (username CHAR(255), hashpass CHAR(255), nif INT);"
        sqlite_cmd.ExecuteNonQuery()
    End Sub


    Private Sub InsertIntoClientLogin()
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        sqlite_conn = New SQLiteConnection("Data Source=login.sqlite;Version=3;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "INSERT INTO Login (username, hashpass, nif) VALUES '" + iusername + "','" + ihashpassword + "'," + Nif.ToString + ";"
        sqlite_cmd.ExecuteNonQuery()
    End Sub

    Private Function ReadHashPassClientLogin(_nome_cliente As String) As Double
        Dim sqlite_conn As SQLiteConnection
        Dim sqlite_cmd As SQLiteCommand
        Dim sqlite_datareader As SQLiteDataReader
        sqlite_conn = New SQLiteConnection("Data Source=database.login;Version=3;New=True;")
        sqlite_conn.Open()
        sqlite_cmd = sqlite_conn.CreateCommand()
        sqlite_cmd.CommandText = "SELECT hashpass FROM Login WHERE nome =" + _nome_cliente + ";"
        sqlite_datareader = sqlite_cmd.ExecuteReader()
        Dim hashreader As String
        Dim SystemAlerts As New SystemAlerts()
        Try
            While (sqlite_datareader.Read())
                hashreader = sqlite_datareader.GetString(0)
            End While
        Catch ex As Exception
            SystemAlerts.ErrorBox(ex)
        End Try
        Return hashreader
    End Function


    '[UPDATE/INSERT]
    Public Function UpdateClienteoginVaules() As String
        Dim SystemAlerts As New SystemAlerts()
        hashisdone = VerifyHash(ihashpassword)
        Try
            If userIsValid = True Then
                InsertIntoClientLogin()
                Return "Concluido!"
            ElseIf (userIsValid = False) Or (hashisdone = False) Or (userIsValid = False And hashisdone = False) Then
                Return "Os dados são invalidos logo o cliente não foi atualizado!"
            End If
        Catch ex As Exception
            Return ex.ToString
            SystemAlerts.ErrorBox(ex)
        End Try
        Return "..."
    End Function
End Class