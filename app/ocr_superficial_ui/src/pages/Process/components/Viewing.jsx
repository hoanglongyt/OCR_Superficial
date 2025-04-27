import { useEffect } from "react"
import { useLocation } from "react-router-dom"
import config from "../../../config"
import BillingBox from "./BillingBox"

export default function Viewing(){
    const location = useLocation()
    const result = location.state?.text
    
    useEffect(() => {
        if(!config.isProduction){
            console.log(result)
        }
    })

    return (
        <BillingBox rawText={result.text}/>
    )
}