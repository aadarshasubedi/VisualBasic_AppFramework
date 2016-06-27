﻿Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Scripting

    Public Module Actives

        <Extension> Public Function DisplType(type As Type) As String
            Dim sb As New StringBuilder

            Call sb.AppendLine($"**Decalre**:  _{type.FullName}_")
            Call sb.AppendLine("Example: ")
            Call sb.AppendLine("```json")
            Call sb.AppendLine(Active(type))
            Call sb.AppendLine("```")

            Return sb.ToString
        End Function

        ''' <summary>
        ''' Display the json of the target type its instance object.
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function Active(type As Type) As String
            Dim obj As Object = type.__active
            Return GetJson(obj, type)
        End Function

        ''' <summary>
        ''' Creates the example instance object for the example
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <Extension> Private Function __active(type As Type) As Object
            If type.Equals(GetType(String)) Then
                Return type.FullName
            End If
            If type.Equals(GetType(Char)) Then
                Return "c"c
            End If
            If type.Equals(GetType(Date)) OrElse type.Equals(GetType(DateTime)) Then
                Return Now
            End If
            If ToStrings.ContainsKey(type) Then
                Return 100.0R
            End If
            If type.IsInheritsFrom(GetType(Array)) Then
                Dim e As Object = type.GetElementType.__active
                Dim array As Array = System.Array.CreateInstance(type.GetElementType, 1)
                array.SetValue(e, Scan0)
                Return array
            End If

            Try
                Dim obj As Object = Activator.CreateInstance(type)

                For Each prop As PropertyInfo In type.GetProperties.Where(
                    Function(x) x.CanWrite AndAlso
                    x.GetIndexParameters.IsNullOrEmpty)

                    Dim value As Object = prop.PropertyType.__active

                    Call prop.SetValue(obj, value)
                Next

                Return obj
            Catch ex As Exception
                ex = New Exception(type.FullName, ex)
                Call App.LogException(ex)
                Return Nothing
            End Try
        End Function
    End Module
End Namespace