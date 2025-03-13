# CSCE 432/632 Project

## Overview

This application was created for Texas A&M University, CSCE 432-500 and CSCE 632-600 in Spring of 2025. The application aims to create an environment through which people can remind their loved ones—targeted at people with
demnentia—to perform their daily tasks. We allow users to enter groups as a sender or a receiver, then allow senders to record and send videos to a receiver. The goal is to make the interface as simple as possible so the
application remains easy to use by everyone. This assists those with dementia, who often have memory troubles and difficulty learning new technologies.

## Requirements

This application is running React Native frontend with an ASP.NET Core backend. Thus, the following are required:
- Visual Studio 2022
- .NET 8.0
- Node 20.5.0
- Rancher Desktop
- Android Studio (for Android deployment)

## Development

1. If you want to develop for Android, install Android Studio. This will fix issues relating to not having access to the `adb` command.
2. Open the project in Visual Studio.
3. If not already running, start Rancher Desktop. You can use Docker Desktop instead, if that's what you prefer.
4. Run the backend on Visual Studio.
5. Open a command prompt in the ClientApp folder.
6. Run the command `npm install`. You only have to do this once.
7. Run the command `npm run android`
