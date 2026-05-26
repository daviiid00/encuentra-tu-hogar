# Authentication Module

## Purpose
Handle authentication and authorization of users.

## Roles
- Tenant
- Landlord
- Admin

## Features
- Register
- Login
- Logout
- JWT Authentication
- Password recovery
- Email verification

## Business Rules
- Email must be unique
- Password must be encrypted
- Only verified users can publish properties

## Security
- JWT tokens
- Password hashing
- Session validation

## Endpoints
POST /auth/register
POST /auth/login
POST /auth/recover-password

## Database Entities
User
Role
Session

## Frontend Requirements
- Maintain current design
- Friendly forms
- Validation messages