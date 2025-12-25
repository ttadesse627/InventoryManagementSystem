'use client'

import Link from 'next/link'
import './globals.css'
import { Inter } from 'next/font/google'
import type { Metadata } from 'next'
 
const inter = Inter({ subsets: ['latin'] })
 
export const metadata: Metadata = {
  title: '404 - Page Not Found',
  description: 'The page you are looking for does not exist.',
}

export default function GlobalNotFoundPage() {
  return (
    <html lang="en" className={inter.className}>
    <head>
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <title>404 - Page Not Found</title>
    </head>
    <body>
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="text-center max-w-lg">
          <div className="mb-8">
            <div className="inline-flex items-center justify-center w-32 h-32 bg-linear-to-br from-purple-100 to-pink-100 rounded-full mb-6">
              <svg className="w-16 h-16 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <h1 className="text-6xl font-bold text-gray-900 mb-4">404</h1>
            <h2 className="text-2xl font-semibold text-gray-700 mb-4">Page not found</h2>
            <p className="text-gray-600 mb-8">
              Sorry, we couldn't find the page you're looking for. Please check the URL or try again later.
            </p>
          </div>
          
          <div className="space-y-4">
            <Link
              href="/warehouse"
              className="inline-block w-full sm:w-auto px-8 py-3 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700 transition-colors shadow hover:shadow-md"
            >
              Return Home
            </Link>
            <div className="text-sm text-gray-500">
              <button
                onClick={() => window.history.back()}
                className="hover:text-gray-700 transition-colors"
              >
                ← Go back
              </button>
            </div>
          </div>
        </div>
      </div>
    </body>
    </html>
    
  )
}