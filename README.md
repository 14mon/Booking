# 📚 Booking System

A robust class booking system built with **.NET 8**, supporting both **OData** and **RESTful** APIs. It uses **PostgreSQL** for data persistence, **Firebase** for authentication, **AWS Lambda + EventBridge** for automated class refund scheduling, and **Redis** for caching transaction history.

---

## 🔧 Tech Stack

| Layer          | Technology               |
| -------------- | ------------------------ |
| Backend API    | .NET 8 (ASP.NET Core)    |
| Database       | PostgreSQL               |
| Authentication | Firebase                 |
| API Style      | REST + OData             |
| Scheduling     | AWS Lambda + EventBridge |
| Caching        | Redis                    |
| Docker         | Postgresql,Redis         |

---

## 📁 Project Structure

## Docker Files

## docker-compose.yml for postgresql db

## redis-docker-compose.yml for Redis Connection

---

## 📦 Environment Variables For API

Create a `.env` file in the root directory with the following content:

Create `firebase.json` file under booking_system folder

I write APIPayload.md for API URL and Payload Json
