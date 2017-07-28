Imports System.Windows.Media.Media3D

Class MainWindow
    Dim bmpWidth As Integer = 1000
    Dim bmpHeight As Integer = 1000
    Dim deImage As New BMP(bmpWidth, bmpHeight)

    Dim deIterations As Integer = 1000

    Dim maxRaySteps As Integer = 100
    Dim minDistance As Double = 0.001

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        genImage()

        deImage.SaveBMP("test.bmp")
    End Sub

    Function DE(ByVal z As Vector3D)
        Dim zTemp As New Vector3D(z.X, z.Y, z.Z)
        Dim dr As Double = 1.0
        Dim r As Double = 0.0

        Dim bailout As Double = 2.0
        Dim power As Double = 8.0

        For i As Integer = 0 To deIterations - 1
            r = zTemp.Length()

            If r > bailout Then
                Exit For
            End If

            Dim theta As Double = Math.Acos(zTemp.Z / r)
            Dim phi As Double = Math.Atan2(zTemp.Y, zTemp.X)

            dr = Math.Pow(r, power - 1.0) * power * dr + 1.0

            Dim zr As Double = Math.Pow(r, power)
            theta *= power
            phi *= power


            zTemp = New Vector3D(Math.Sin(theta) * Math.Cos(phi), Math.Sin(phi) * Math.Sin(theta), Math.Cos(theta)) * zr
            zTemp += z
        Next

        Return 0.5 * Math.Log(r) * r / dr
    End Function

    Function RayMarch(ByVal from As Vector3D, ByVal direction As Vector3D)
        Dim totalDistance As Double = 0.0

        Dim steps As Integer = 0

        Dim p As New Vector3D
        For steps = 0 To maxRaySteps - 1
            p = from + totalDistance * direction
            Dim distance As Double = DE(p)
            totalDistance += distance
            If distance < minDistance Then
                Exit For
            End If
        Next

        Return 1.0 - CDbl(steps) / CDbl(maxRaySteps)
    End Function

    Function getLookRay(ByVal lookDir As Vector3D, ByVal fov As Double, ByVal xNorm As Double, ByVal yNorm As Double)
        Dim theta As Double = Math.Atan2(lookDir.Y, lookDir.X)
        Dim phi As Double = Math.Atan(New Vector(lookDir.X, lookDir.Y).Length / lookDir.Z)

        ' fov in radians
        theta += (fov / 2.0) * xNorm
        phi += ((fov / 2.0) * yNorm)

        Return New Vector3D(1.0 * Math.Sin(phi) * Math.Cos(theta), 1.0 * Math.Sin(phi) * Math.Sin(theta), 1.0 * Math.Cos(phi))
    End Function

    Sub genImage()
        Dim cameraPos As New Vector3D(-3.0, 3.0, 0.0)
        Dim lookAt As New Vector3D(0, 0, 0)
        Dim lookDir As Vector3D = (lookAt - cameraPos)
        Dim fov As Double = Math.PI / 4.0

        For x As Integer = 0 To bmpWidth - 1
            For y As Integer = 0 To bmpHeight - 1
                Dim xNorm As Double = ((x / CDbl(bmpWidth)) * 2.0) - 1
                Dim yNorm As Double = ((y / CDbl(bmpHeight)) * 2.0) - 1

                Dim lookRay As Vector3D = getLookRay(lookDir, fov, xNorm, yNorm)

                Dim dist As Double = RayMarch(cameraPos, lookRay)

                Dim bright As Byte = (dist * 255.0)
                deImage.SetPixel(x, y, bright, bright, bright)
            Next
            Console.WriteLine((x * 100) / bmpWidth)
        Next
    End Sub
End Class
