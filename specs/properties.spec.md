# Properties Module

## Purpose
Manage rental properties inside the platform.

## Features
- Create property
- Edit property
- Delete property
- Upload images
- Set availability
- Add location
- Add property details

## Property Information
- Title
- Description
- Price
- City
- Neighborhood
- Property type
- Services
- Images

## Business Rules
- Only landlords can publish
- Properties require valid location
- Images must be validated

## Endpoints
GET /properties
POST /properties
PUT /properties/{id}
DELETE /properties/{id}

## Frontend Requirements
- Current UI must remain unchanged
- Responsive property cards
- Filters panel