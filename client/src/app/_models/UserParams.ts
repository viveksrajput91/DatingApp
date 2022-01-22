import { User } from "./user";

export class UserParams{
    pageNumber:Number=1;
    pageSize:Number=5;
    minAge:Number=18;
    maxAge:Number=99;
    gender:string;
    orderBy='lastActive';
    currentUserName:string;

    constructor(private user:User)
    {
        this.gender=user.gender==="male"?"female":"male";
        this.currentUserName=user.username;
    }
}