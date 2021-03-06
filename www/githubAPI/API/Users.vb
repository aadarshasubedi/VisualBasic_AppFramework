﻿#Region "Microsoft.VisualBasic::aab90eff4a63d4afa6ec6fd6ed42fad7, ..\visualbasic_App\www\githubAPI\API\Users.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Webservices.Github.Class

Namespace API

    Public Module Users

        ''' <summary>
        ''' Get a single user
        ''' </summary>
        ''' <param name="username"></param>
        ''' <returns></returns>
        Public Function GetUser(username As String) As User
            Dim url As String = GithubAPI & $"/users/{username}"
            Dim json As String = url.GetRequest(https:=True)
            Dim user As User = json.LoadObject(Of User)
            Return user
        End Function

        Public Function Followers(username As String) As User()
            Dim url As String = GithubAPI & $"/users/{username}/followers"
            Dim json As String = url.GetRequest(https:=True)
            Dim users As User() = json.LoadObject(Of User())
            Return users
        End Function

        Public Function Following(username As String) As User()
            Dim url As String = GithubAPI & $"/users/{username}/following"
            Dim json As String = url.GetRequest(https:=True)
            Dim users As User() = json.LoadObject(Of User())
            Return users
        End Function
    End Module
End Namespace
