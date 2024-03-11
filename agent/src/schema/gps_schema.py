from marshmallow import Schema, fields


class GpsSchema(Schema):
    longitude = fields.Number()
    latitude = fields.Number()
