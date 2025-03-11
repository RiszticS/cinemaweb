# Mozi Időpontfoglaló Rendszer

## 1. Projekt bemutatása
A gyakorlat keretében elkezdünk egy **ASP.NET Core MVC** projektet. Az alkalmazás két fő komponensből fog állni:
- **Web projekt**: itt kapnak helyet a **Controllerek** és a felhasználói felület.
- **DataAccess projekt**: ez tartalmazza az **adatbázis kezelést** és az **üzleti logikát**.

Az adatbázis kezeléshez az **Entity Framework Core** keretrendszert fogjuk használni, valamint a **code-first** elv alapján hozzuk létre az adatbázist.

Az alkalmazás egy **mozi időpontfoglaló rendszer** lesz, amely lehetőséget biztosít a felhasználók számára, hogy megtekintsék a filmeket és azok részleteit.

**Funkciók:**
- **Főoldal**
- **Filmek listája**
- **Film részleteinek megtekintése**

## 1.1 Fejlesztői környezet
A fejlesztéshez az alábbi szoftverekre lesz szükség:
- **Visual Studio 2022** (vagy frissítés a legújabb verzióra a **Visual Studio Installer** segítségével)
- Az **ASP.NET and web development** workload telepítése Visual Studio-ban

**Tipp:** Az **ELTE IK** hallgatói ingyenesen hozzájuthatnak a **Visual Studio 2022 Enterprise** licenchez az alábbi linken:  
[https://aka.ms/devtoolsforteaching](https://aka.ms/devtoolsforteaching)

Alternatív fejlesztői környezetként használható a **JetBrains Rider**, amelyhez szintén ingyenes licenc igényelhető az ELTE hallgatók számára.

## 1.2 Projekt létrehozása
A projekt indításához kövessük az alábbi lépéseket:

1. Nyissuk meg a **Visual Studio 2022**-t.
2. Válasszuk a **File → New → Project** menüpontot.
3. Kereséssel válasszuk ki az **ASP.NET Core Web Application (Model-View-Controller)** sablont.
4. Fontos, hogy **.NET (Core)** projektet válasszunk, ne **.NET Framework**-ot!
5. A projekt neve legyen: **Cinema.Web**, a **Solution neve** pedig: **Cinema**.
6. A .NET verzió kiválasztásakor válasszuk a **.NET 8** verziót.
7. Az SSL (HTTPS) támogatáshoz a rendszer tanúsítványt telepít, ha ez hibába ütközik, akkor kapcsoljuk ki a HTTPS konfigurációt.

A sikeres létrehozás után a fejlesztést a projektstruktúra kialakításával folytatjuk.

